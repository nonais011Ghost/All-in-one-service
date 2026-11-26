using System;
using System.Data;
using System.Data.SqlClient;
using AIOSICT011.Models;

namespace AIOSICT011.DataAccess
{
    // Reads and saves the Transactions table (the service jobs).
    // Each UPDATE checks the current Status in its WHERE clause, so a job cannot skip a step.
    public static class JobData
    {
        // A customer requests a provider: returns the new TransactionID
        public static int Insert(int customerId, int providerId)
        {
            object id = Db.GetValue(
                "INSERT INTO Transactions (CustomerID, ProviderID, ServiceDate) VALUES (@c, @p, @d); SELECT SCOPE_IDENTITY();",
                new SqlParameter("@c", customerId),
                new SqlParameter("@p", providerId),
                new SqlParameter("@d", DateTime.Now));
            return Convert.ToInt32(id);
        }

        public static ServiceTransaction GetById(int transactionId)
        {
            DataTable dt = Db.GetData("SELECT * FROM Transactions WHERE TransactionID = @id",
                new SqlParameter("@id", transactionId));
            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];
            ServiceTransaction job = new ServiceTransaction();
            job.TransactionID = Convert.ToInt32(row["TransactionID"]);
            job.CustomerID = Convert.ToInt32(row["CustomerID"]);
            job.ProviderID = Convert.ToInt32(row["ProviderID"]);
            job.ServiceDate = Convert.ToDateTime(row["ServiceDate"]);
            job.Status = Convert.ToString(row["Status"]);
            job.CustomerPaid = Convert.ToBoolean(row["CustomerPaid"]);
            job.ProviderPaid = Convert.ToBoolean(row["ProviderPaid"]);
            job.RedFlag = Convert.ToBoolean(row["RedFlag"]);
            job.InvoiceNo = Convert.ToString(row["InvoiceNo"]);
            job.Feedback = Convert.ToString(row["Feedback"]);

            // these columns can be empty (NULL) until the job reaches that step
            if (row["HoursWorked"] != DBNull.Value) job.HoursWorked = Convert.ToDecimal(row["HoursWorked"]);
            if (row["Amount"] != DBNull.Value) job.Amount = Convert.ToDecimal(row["Amount"]);
            if (row["Rating"] != DBNull.Value) job.Rating = Convert.ToInt32(row["Rating"]);
            return job;
        }

        // Provider accepts the job
        public static int SetInProgress(int transactionId)
        {
            return Db.Execute(
                "UPDATE Transactions SET Status = 'In Progress' WHERE TransactionID = @id AND Status = 'Requested'",
                new SqlParameter("@id", transactionId));
        }

        // Provider finishes the job: hours, amount and invoice number are saved
        public static int SetCompleted(int transactionId, decimal hours, decimal amount, string invoiceNo)
        {
            return Db.Execute(
                "UPDATE Transactions SET Status = 'Completed', HoursWorked = @h, Amount = @a, InvoiceNo = @i " +
                "WHERE TransactionID = @id AND Status = 'In Progress'",
                new SqlParameter("@h", hours),
                new SqlParameter("@a", amount),
                new SqlParameter("@i", invoiceNo),
                new SqlParameter("@id", transactionId));
        }

        // Customer pays AIOS
        public static int SetPaid(int transactionId)
        {
            return Db.Execute(
                "UPDATE Transactions SET Status = 'Paid', CustomerPaid = 1 WHERE TransactionID = @id AND Status = 'Completed'",
                new SqlParameter("@id", transactionId));
        }

        // AIOS pays the provider (only after the customer has paid)
        public static int SetClosed(int transactionId)
        {
            return Db.Execute(
                "UPDATE Transactions SET Status = 'Closed', ProviderPaid = 1 " +
                "WHERE TransactionID = @id AND Status = 'Paid' AND CustomerPaid = 1",
                new SqlParameter("@id", transactionId));
        }

        public static void SaveFeedback(int transactionId, int rating, string feedback, bool redFlag)
        {
            Db.Execute(
                "UPDATE Transactions SET Rating = @r, Feedback = @f, RedFlag = @flag WHERE TransactionID = @id",
                new SqlParameter("@r", rating),
                new SqlParameter("@f", feedback),
                new SqlParameter("@flag", redFlag),
                new SqlParameter("@id", transactionId));
        }

        // 0 means "do not filter on this": (0, 0) = every job, (5, 0) = customer 5, (0, 2) = provider 2
        public static DataTable GetJobs(int customerId, int providerId)
        {
            return Db.GetData(
                "SELECT t.TransactionID, c.FullName AS Customer, p.FullName AS Provider, t.ServiceDate, " +
                "t.HoursWorked, t.Status, t.Amount, t.InvoiceNo, t.CustomerPaid, t.ProviderPaid, t.Rating, t.RedFlag " +
                "FROM Transactions t " +
                "INNER JOIN Customers c ON t.CustomerID = c.CustomerID " +
                "INNER JOIN ServiceProviders p ON t.ProviderID = p.ProviderID " +
                "WHERE (@c = 0 OR t.CustomerID = @c) AND (@p = 0 OR t.ProviderID = @p) " +
                "ORDER BY t.ServiceDate DESC",
                new SqlParameter("@c", customerId),
                new SqlParameter("@p", providerId));
        }
    }
}
