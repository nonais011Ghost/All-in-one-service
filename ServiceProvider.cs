using System;

namespace AIOSICT011.Models
{
    // Subclass inheriting from Person and implementing IReportable
    public class ServiceProvider : Person, IReportable
    {
        public int ProviderID { get; set; }
        public string BusinessRegNo { get; set; }
        public int ServiceTypeID { get; set; }
        public string ServiceTypeName { get; set; }
        public decimal HourlyRate { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool IsBlacklisted { get; set; } = false;
        public int RedFlags { get; set; } = 0;
        public decimal AvgRating { get; set; } = 0m;   // 0 = no ratings yet

        public ServiceProvider() { }

        public ServiceProvider(string fullName, string phone, string address, int regionID,
                               string businessRegNo, int serviceTypeID, decimal hourlyRate)
            : base(fullName, phone, address, regionID)
        {
            BusinessRegNo = businessRegNo;
            ServiceTypeID = serviceTypeID;
            HourlyRate = hourlyRate;
        }

        public override string GetRole()
        {
            return "Service Provider";
        }

        // Overriding virtual method (polymorphism)
        public override string DisplayInfo()
        {
            string status;
            if (IsBlacklisted)
            {
                status = "Blacklisted";
            }
            else if (IsAvailable)
            {
                status = "Available";
            }
            else
            {
                status = "Busy";
            }

            return base.DisplayInfo() + " | Rate: P" + HourlyRate + "/hr | Status: " + status;
        }

        // A business rule kept inside the class that owns the data
        public bool CanAcceptJobs()
        {
            return IsAvailable && !IsBlacklisted;
        }

        public string GetSummaryText()
        {
            return "Provider #" + ProviderID + ": " + FullName + " | Rate: P" + HourlyRate + " | Rating: " + AvgRating;
        }
    }
}
