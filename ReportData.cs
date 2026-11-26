using System;
using System.Data;

namespace AIOSICT011.DataAccess
{
    public static class ReportData
    {
        public static DataTable ProviderInventory()
        {
            return Db.GetData(
                "SELECT p.ProviderID, p.FullName, st.TypeName AS ServiceType, p.BusinessRegNo, p.Phone, " +
                "r.Ward, r.Block, p.HourlyRate, p.AvgRating, p.RedFlags, " +
                "CASE WHEN p.IsBlacklisted = 1 THEN 'Blacklisted' WHEN p.IsAvailable = 1 THEN 'Available' ELSE 'Busy' END AS Status " +
                "FROM ServiceProviders p " +
                "INNER JOIN ServiceTypes st ON p.ServiceTypeID = st.ServiceTypeID " +
                "INNER JOIN Regions r ON p.RegionID = r.RegionID");
        }

        // Jobs that have been completed (service records)
        public static DataTable ServiceRecords()
        {
            return Db.GetData(
                "SELECT t.TransactionID, p.FullName AS Provider, st.TypeName AS ServiceType, c.FullName AS Customer, " +
                "t.ServiceDate, t.HoursWorked, t.Amount, t.Rating, t.Feedback, t.RedFlag " +
                "FROM Transactions t " +
                "INNER JOIN ServiceProviders p ON t.ProviderID = p.ProviderID " +
                "INNER JOIN ServiceTypes st ON p.ServiceTypeID = st.ServiceTypeID " +
                "INNER JOIN Customers c ON t.CustomerID = c.CustomerID " +
                "WHERE t.Status IN ('Completed', 'Paid', 'Closed')");
        }

        public static DataTable JobHistory()
        {
            return JobData.GetJobs(0, 0);
        }

        // Summary numbers: a table with two columns (Metric and Value)
        public static DataTable Statistics()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Metric");
            dt.Columns.Add("Value");

            dt.Rows.Add("Total customers", Show("SELECT COUNT(*) FROM Customers"));
            dt.Rows.Add("Total service providers", Show("SELECT COUNT(*) FROM ServiceProviders"));
            dt.Rows.Add("Available providers", Show("SELECT COUNT(*) FROM ServiceProviders WHERE IsAvailable = 1 AND IsBlacklisted = 0"));
            dt.Rows.Add("Blacklisted providers", Show("SELECT COUNT(*) FROM ServiceProviders WHERE IsBlacklisted = 1"));
            dt.Rows.Add("Total jobs", Show("SELECT COUNT(*) FROM Transactions"));
            dt.Rows.Add("Jobs paid or closed", Show("SELECT COUNT(*) FROM Transactions WHERE Status IN ('Paid', 'Closed')"));
            dt.Rows.Add("Total invoiced (P)", Show("SELECT ISNULL(SUM(Amount), 0) FROM Transactions WHERE Status IN ('Completed', 'Paid', 'Closed')"));
            dt.Rows.Add("Received from customers (P)", Show("SELECT ISNULL(SUM(Amount), 0) FROM Transactions WHERE CustomerPaid = 1"));
            dt.Rows.Add("Paid out to providers (P)", Show("SELECT ISNULL(SUM(Amount), 0) FROM Transactions WHERE ProviderPaid = 1"));
            dt.Rows.Add("Average rating", Show("SELECT ISNULL(CAST(AVG(CAST(Rating AS DECIMAL(5,2))) AS DECIMAL(4,2)), 0) FROM Transactions WHERE Rating IS NOT NULL"));
            return dt;
        }

        // Runs a query that returns one value and turns it into text
        private static string Show(string sql)
        {
            object value = Db.GetValue(sql);
            if (value == null || value == DBNull.Value)
                return "-";
            return Convert.ToString(value);
        }
    }
}
