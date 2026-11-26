using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using AIOSICT011.DataAccess;
using AIOSICT011.Models;

namespace AIOSICT011.Business
{
    // Business logic tier: the rules of AIOS live here.
    // The forms call this class, and this class calls the DataAccess classes (no SQL in this file).
    public static class ServiceManager
    {
        // A provider with this many red flags is blacklisted automatically
        public const int MaxRedFlags = 3;

        // ---------------------------------------------------------------
        // Passwords and login
        // ---------------------------------------------------------------

        // Passwords are saved as a SHA-256 hash, never as plain text
        public static string HashPassword(string password)
        {
            SHA256 sha = SHA256.Create();
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        // Returns the logged in user, or null if the details are wrong
        public static User Login(string username, string password)
        {
            if (!Validator.IsRequired(username) || !Validator.IsRequired(password))
                throw new ArgumentException("Enter both username and password.");

            return UserData.Find(username.Trim(), HashPassword(password));
        }

        // ---------------------------------------------------------------
        // Registration and customer update
        // ---------------------------------------------------------------

        // Checks the details that customers and providers both have
        private static void CheckCommon(List<string> errors, string fullName, string phone, int regionId,
                                        string username, string password, string confirm)
        {
            if (!Validator.IsRequired(fullName)) errors.Add("Full name is required.");
            if (!Validator.IsValidPhone(phone)) errors.Add("Enter a valid phone number (7 or 8 digits, may start with +267).");
            if (regionId <= 0) errors.Add("Select a region.");

            if (!Validator.IsValidUsername(username))
                errors.Add("Username must be 4 to 20 letters, digits or underscores.");
            else if (UserData.UsernameExists(username.Trim()))
                errors.Add("That username is already taken.");

            if (!Validator.IsValidPassword(password)) errors.Add("Password must be at least 6 characters.");
            if (password != confirm) errors.Add("Passwords do not match.");
        }

        // Stops the process and shows every problem at once
        private static void ThrowIfErrors(List<string> errors)
        {
            if (errors.Count > 0)
                throw new ArgumentException(string.Join(Environment.NewLine, errors.ToArray()));
        }

        // Returns the new membership ID
        public static string RegisterCustomer(string fullName, string phone, string address, string email,
                                              int regionId, string username, string password, string confirm)
        {
            List<string> errors = new List<string>();
            CheckCommon(errors, fullName, phone, regionId, username, password, confirm);
            if (!Validator.IsValidEmail(email)) errors.Add("Enter a valid email address.");
            ThrowIfErrors(errors);

            Customer c = new Customer(fullName, phone, address.Trim(), regionId, email.Trim());
            c.MembershipID = "AIOS-" + DateTime.Now.ToString("yyMMddHHmmss");

            int customerId = CustomerData.Insert(c);
            try
            {
                UserData.Insert(username.Trim(), HashPassword(password), User.CustomerRole, customerId);
            }
            catch
            {
                // if the login could not be saved, remove the customer so no half-registered customer is left
                CustomerData.Delete(customerId);
                throw;
            }
            return c.MembershipID;
        }

        public static void RegisterProvider(string fullName, string phone, string address, int regionId,
                                            string businessRegNo, int serviceTypeId, decimal hourlyRate,
                                            string username, string password, string confirm)
        {
            List<string> errors = new List<string>();
            CheckCommon(errors, fullName, phone, regionId, username, password, confirm);
            if (!Validator.IsValidBusinessReg(businessRegNo)) errors.Add("Enter a valid business registration number (5 to 20 letters, digits, - or /).");
            if (serviceTypeId <= 0) errors.Add("Select a service type.");
            if (hourlyRate <= 0) errors.Add("Hourly rate must be greater than zero.");
            ThrowIfErrors(errors);

            ServiceProvider p = new ServiceProvider(fullName, phone, address.Trim(), regionId,
                                                    businessRegNo.Trim(), serviceTypeId, hourlyRate);

            int providerId = ProviderData.Insert(p);
            try
            {
                UserData.Insert(username.Trim(), HashPassword(password), User.ProviderRole, providerId);
            }
            catch
            {
                ProviderData.Delete(providerId);
                throw;
            }
        }

        public static void UpdateCustomer(int customerId, string fullName, string phone, string address, string email)
        {
            List<string> errors = new List<string>();
            if (customerId <= 0) errors.Add("Select a customer first.");
            if (!Validator.IsRequired(fullName)) errors.Add("Full name is required.");
            if (!Validator.IsValidPhone(phone)) errors.Add("Enter a valid phone number.");
            if (!Validator.IsValidEmail(email)) errors.Add("Enter a valid email address.");
            ThrowIfErrors(errors);

            CustomerData.Update(customerId, fullName.Trim(), phone.Trim(), address.Trim(), email.Trim());
        }

        // ---------------------------------------------------------------
        // Lists for the screens
        // ---------------------------------------------------------------

        public static DataTable GetServiceTypes(bool includeAll) { return LookupData.GetServiceTypes(includeAll); }

        public static DataTable GetRegions(bool includeAll) { return LookupData.GetRegions(includeAll); }

        public static DataTable SearchProviders(string keyword, int serviceTypeId, int regionId, bool onlyAvailable)
        {
            return ProviderData.Search(keyword, serviceTypeId, regionId, onlyAvailable);
        }

        public static DataTable GetCustomers()
        {
            return CustomerData.GetAll();
        }

        public static DataTable GetCustomerHistory(int customerId)
        {
            return JobData.GetJobs(customerId, 0);
        }

        // Customers see their own jobs, providers see their own jobs, the admin sees all jobs
        public static DataTable GetJobsForCurrentUser()
        {
            if (CurrentSession.IsCustomer)
                return JobData.GetJobs(CurrentSession.LoggedInUser.PersonID, 0);
            if (CurrentSession.IsProvider)
                return JobData.GetJobs(0, CurrentSession.LoggedInUser.PersonID);
            return JobData.GetJobs(0, 0);
        }

        // Admin changes a provider between Available and Busy
        public static void ToggleAvailability(int providerId)
        {
            ServiceProvider p = ProviderData.GetById(providerId);
            if (p == null)
                throw new ArgumentException("Provider not found.");
            if (p.IsBlacklisted)
                throw new InvalidOperationException("Blacklisted providers cannot be made available.");

            ProviderData.SetAvailability(providerId, !p.IsAvailable);
        }

        // ---------------------------------------------------------------
        // The job workflow: Requested -> In Progress -> Completed -> Paid -> Closed
        // ---------------------------------------------------------------

        // Job total = hourly rate x hours worked
        public static decimal CalculateJobTotal(decimal hourlyRate, decimal hours)
        {
            if (hours <= 0)
                throw new ArgumentException("Hours worked must be greater than zero.");
            return Math.Round(hourlyRate * hours, 2);
        }

        public static bool ShouldBlacklist(int redFlags)
        {
            return redFlags >= MaxRedFlags;
        }

        // 1. Customer requests a specific provider. Returns the job number.
        public static int RequestJob(int customerId, int providerId)
        {
            ServiceProvider p = ProviderData.GetById(providerId);
            if (p == null)
                throw new ArgumentException("Provider not found.");
            if (p.IsBlacklisted)
                throw new InvalidOperationException("This provider is blacklisted and cannot be requested.");
            if (!p.CanAcceptJobs())
                throw new InvalidOperationException("This provider is busy. Please choose another provider.");

            return JobData.Insert(customerId, providerId);
        }

        // 2. Provider accepts the job and becomes busy
        public static void AcceptJob(int jobId)
        {
            ServiceTransaction job = GetJob(jobId);
            int rows = JobData.SetInProgress(jobId);
            if (rows == 0)
                throw new InvalidOperationException("Only a requested job can be accepted.");

            ProviderData.SetAvailability(job.ProviderID, false);
        }

        // 3. Provider completes the job: total and invoice are created, provider is available again
        public static decimal CompleteJob(int jobId, decimal hours)
        {
            ServiceTransaction job = GetJob(jobId);
            ServiceProvider p = ProviderData.GetById(job.ProviderID);

            decimal total = CalculateJobTotal(p.HourlyRate, hours);
            string invoiceNo = "INV-" + jobId.ToString("00000");

            int rows = JobData.SetCompleted(jobId, hours, total, invoiceNo);
            if (rows == 0)
                throw new InvalidOperationException("Only a job in progress can be completed.");

            ProviderData.SetAvailability(job.ProviderID, true);
            return total;
        }

        // 4. Customer pays AIOS
        public static void PayJob(int jobId)
        {
            GetJob(jobId);
            int rows = JobData.SetPaid(jobId);
            if (rows == 0)
                throw new InvalidOperationException("Only a completed job can be paid.");
        }

        // 5. AIOS pays the provider (after the customer has paid)
        public static void PayProvider(int jobId)
        {
            GetJob(jobId);
            int rows = JobData.SetClosed(jobId);
            if (rows == 0)
                throw new InvalidOperationException("The customer must pay AIOS before the provider is paid.");
        }

        // Rating, feedback and red flag. Returns true if the provider was blacklisted because of it.
        public static bool SubmitFeedback(int jobId, int rating, string feedback, bool redFlag)
        {
            ServiceTransaction job = GetJob(jobId);

            if (job.Status != TransactionStatus.Paid && job.Status != TransactionStatus.Closed)
                throw new InvalidOperationException("You can rate a job only after it has been paid.");
            if (job.Rating > 0)
                throw new InvalidOperationException("This job has already been rated.");
            if (!Validator.IsValidRating(rating))
                throw new ArgumentException("Rating must be between 1 and 5.");
            if (feedback == null) feedback = "";
            if (feedback.Length > 300)
                throw new ArgumentException("Feedback must be 300 characters or fewer.");

            JobData.SaveFeedback(jobId, rating, feedback, redFlag);
            ProviderData.RecalculateStats(job.ProviderID);

            int flags = ProviderData.GetRedFlags(job.ProviderID);
            if (ShouldBlacklist(flags))
            {
                ProviderData.Blacklist(job.ProviderID);
                return true;
            }
            return false;
        }

        // Finds a job or stops with a clear message
        private static ServiceTransaction GetJob(int jobId)
        {
            ServiceTransaction job = JobData.GetById(jobId);
            if (job == null)
                throw new ArgumentException("Select a job first.");
            return job;
        }

        // ---------------------------------------------------------------
        // Reports
        // ---------------------------------------------------------------

        public static readonly string[] ReportNames =
            { "Provider Inventory", "Service Records", "Job History", "Statistics", "People List (Polymorphism)" };

        public static DataTable GetReport(string reportName)
        {
            if (reportName == "Provider Inventory") return ReportData.ProviderInventory();
            if (reportName == "Service Records") return ReportData.ServiceRecords();
            if (reportName == "Job History") return ReportData.JobHistory();
            if (reportName == "Statistics") return ReportData.Statistics();
            if (reportName == "People List (Polymorphism)") return GetPeopleTable();
            throw new ArgumentException("Choose a report.");
        }

        // A List<Person> holds both customers and providers (inheritance, Lesson 6 and 7)
        public static List<Person> GetPeople()
        {
            List<Person> people = new List<Person>();

            foreach (Customer c in CustomerData.GetCustomerList())
            {
                people.Add(c);
            }
            foreach (ServiceProvider p in ProviderData.GetProviderList())
            {
                people.Add(p);
            }
            return people;
        }

        // Calls DisplayInfo() on every person: each class answers in its own way (polymorphism)
        public static DataTable GetPeopleTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Role");
            dt.Columns.Add("Details");

            foreach (Person person in GetPeople())
            {
                dt.Rows.Add(person.GetRole(), person.DisplayInfo());
            }
            return dt;
        }
    }
}
