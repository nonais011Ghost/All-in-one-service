using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AIOSICT011.Models;

namespace AIOSICT011.DataAccess
{
    public static class ProviderData
    {
        // Saves a provider and returns the new ProviderID
        public static int Insert(ServiceProvider p)
        {
            string sql = "INSERT INTO ServiceProviders (FullName, Phone, Address, BusinessRegNo, RegionID, ServiceTypeID, HourlyRate) " +
                         "VALUES (@n, @p, @a, @b, @r, @s, @h); SELECT SCOPE_IDENTITY();";
            object id = Db.GetValue(sql,
                new SqlParameter("@n", p.FullName),
                new SqlParameter("@p", p.Phone),
                new SqlParameter("@a", p.Address),
                new SqlParameter("@b", p.BusinessRegNo),
                new SqlParameter("@r", p.RegionID),
                new SqlParameter("@s", p.ServiceTypeID),
                new SqlParameter("@h", p.HourlyRate));
            return Convert.ToInt32(id);
        }

        public static void Delete(int providerId)
        {
            Db.Execute("DELETE FROM ServiceProviders WHERE ProviderID = @id", new SqlParameter("@id", providerId));
        }

        // Catalogue: search by keyword and filter by service type, region and availability (0 = all)
        public static DataTable Search(string keyword, int serviceTypeId, int regionId, bool onlyAvailable)
        {
            string sql =
                "SELECT p.ProviderID, p.FullName, st.TypeName AS ServiceType, " +
                "r.Unit + ' / ' + r.Ward + ' / ' + r.Block AS Region, p.Phone, p.HourlyRate, p.AvgRating, p.RedFlags, " +
                "CASE WHEN p.IsBlacklisted = 1 THEN 'Blacklisted' WHEN p.IsAvailable = 1 THEN 'Available' ELSE 'Busy' END AS Status " +
                "FROM ServiceProviders p " +
                "INNER JOIN ServiceTypes st ON p.ServiceTypeID = st.ServiceTypeID " +
                "INNER JOIN Regions r ON p.RegionID = r.RegionID " +
                "WHERE (@kw = '' OR p.FullName LIKE '%' + @kw + '%' OR p.Address LIKE '%' + @kw + '%') " +
                "AND (@st = 0 OR p.ServiceTypeID = @st) " +
                "AND (@rg = 0 OR p.RegionID = @rg) " +
                "AND (@av = 0 OR (p.IsAvailable = 1 AND p.IsBlacklisted = 0)) " +
                "ORDER BY p.AvgRating DESC, p.FullName";

            int available = 0;
            if (onlyAvailable) available = 1;

            return Db.GetData(sql,
                new SqlParameter("@kw", keyword),
                new SqlParameter("@st", serviceTypeId),
                new SqlParameter("@rg", regionId),
                new SqlParameter("@av", available));
        }

        // Turns one database row into a ServiceProvider object
        private static ServiceProvider RowToProvider(DataRow row)
        {
            ServiceProvider p = new ServiceProvider();
            p.ProviderID = Convert.ToInt32(row["ProviderID"]);
            p.FullName = Convert.ToString(row["FullName"]);
            p.Phone = Convert.ToString(row["Phone"]);
            p.Address = Convert.ToString(row["Address"]);
            p.RegionID = Convert.ToInt32(row["RegionID"]);
            p.BusinessRegNo = Convert.ToString(row["BusinessRegNo"]);
            p.ServiceTypeID = Convert.ToInt32(row["ServiceTypeID"]);
            p.ServiceTypeName = Convert.ToString(row["TypeName"]);
            p.HourlyRate = Convert.ToDecimal(row["HourlyRate"]);
            p.IsAvailable = Convert.ToBoolean(row["IsAvailable"]);
            p.IsBlacklisted = Convert.ToBoolean(row["IsBlacklisted"]);
            p.RedFlags = Convert.ToInt32(row["RedFlags"]);
            p.AvgRating = Convert.ToDecimal(row["AvgRating"]);
            return p;
        }

        private const string SelectProvider =
            "SELECT p.*, st.TypeName FROM ServiceProviders p " +
            "INNER JOIN ServiceTypes st ON p.ServiceTypeID = st.ServiceTypeID ";

        public static ServiceProvider GetById(int providerId)
        {
            DataTable dt = Db.GetData(SelectProvider + "WHERE p.ProviderID = @id", new SqlParameter("@id", providerId));
            if (dt.Rows.Count == 0)
                return null;
            return RowToProvider(dt.Rows[0]);
        }

        // For the polymorphism report: a List of ServiceProvider objects (Lesson 7)
        public static List<ServiceProvider> GetProviderList()
        {
            List<ServiceProvider> list = new List<ServiceProvider>();
            DataTable dt = Db.GetData(SelectProvider + "ORDER BY p.FullName");
            foreach (DataRow row in dt.Rows)
            {
                list.Add(RowToProvider(row));
            }
            return list;
        }

        // A blacklisted provider can never be made available again
        public static void SetAvailability(int providerId, bool available)
        {
            Db.Execute("UPDATE ServiceProviders SET IsAvailable = @a WHERE ProviderID = @id AND IsBlacklisted = 0",
                new SqlParameter("@a", available),
                new SqlParameter("@id", providerId));
        }

        // Recalculates the average rating and the red flag count from the Transactions table
        public static void RecalculateStats(int providerId)
        {
            string sql =
                "UPDATE ServiceProviders SET " +
                "AvgRating = ISNULL((SELECT AVG(CAST(Rating AS DECIMAL(5,2))) FROM Transactions " +
                "                    WHERE ProviderID = @id AND Rating IS NOT NULL), 0), " +
                "RedFlags = (SELECT COUNT(*) FROM Transactions WHERE ProviderID = @id AND RedFlag = 1) " +
                "WHERE ProviderID = @id";
            Db.Execute(sql, new SqlParameter("@id", providerId));
        }

        public static int GetRedFlags(int providerId)
        {
            object flags = Db.GetValue("SELECT RedFlags FROM ServiceProviders WHERE ProviderID = @id",
                new SqlParameter("@id", providerId));
            return Convert.ToInt32(flags);
        }

        public static void Blacklist(int providerId)
        {
            Db.Execute("UPDATE ServiceProviders SET IsBlacklisted = 1, IsAvailable = 0 WHERE ProviderID = @id",
                new SqlParameter("@id", providerId));
        }
    }
}
