using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AIOSICT011.Models;

namespace AIOSICT011.DataAccess
{
    public static class CustomerData
    {
        // Saves a customer and returns the new CustomerID
        public static int Insert(Customer c)
        {
            string sql = "INSERT INTO Customers (MembershipID, FullName, Phone, Email, Address, RegionID) " +
                         "VALUES (@m, @n, @p, @e, @a, @r); SELECT SCOPE_IDENTITY();";
            object id = Db.GetValue(sql,
                new SqlParameter("@m", c.MembershipID),
                new SqlParameter("@n", c.FullName),
                new SqlParameter("@p", c.Phone),
                new SqlParameter("@e", c.Email),
                new SqlParameter("@a", c.Address),
                new SqlParameter("@r", c.RegionID));
            return Convert.ToInt32(id);
        }

        public static void Delete(int customerId)
        {
            Db.Execute("DELETE FROM Customers WHERE CustomerID = @id", new SqlParameter("@id", customerId));
        }

        public static void Update(int customerId, string fullName, string phone, string address, string email)
        {
            Db.Execute(
                "UPDATE Customers SET FullName = @n, Phone = @p, Address = @a, Email = @e WHERE CustomerID = @id",
                new SqlParameter("@n", fullName),
                new SqlParameter("@p", phone),
                new SqlParameter("@a", address),
                new SqlParameter("@e", email),
                new SqlParameter("@id", customerId));
        }

        // For the customers grid
        public static DataTable GetAll()
        {
            return Db.GetData(
                "SELECT c.CustomerID, c.MembershipID, c.FullName, c.Phone, c.Email, c.Address, " +
                "r.Unit + ' / ' + r.Ward + ' / ' + r.Block AS Region " +
                "FROM Customers c INNER JOIN Regions r ON c.RegionID = r.RegionID ORDER BY c.FullName");
        }

        // For the polymorphism report: a List of Customer objects (Lesson 7)
        public static List<Customer> GetCustomerList()
        {
            List<Customer> list = new List<Customer>();
            DataTable dt = Db.GetData(
                "SELECT CustomerID, MembershipID, FullName, Phone, Email, Address, RegionID FROM Customers ORDER BY FullName");

            foreach (DataRow row in dt.Rows)
            {
                Customer c = new Customer();
                c.CustomerID = Convert.ToInt32(row["CustomerID"]);
                c.MembershipID = Convert.ToString(row["MembershipID"]);
                c.FullName = Convert.ToString(row["FullName"]);
                c.Phone = Convert.ToString(row["Phone"]);
                c.Email = Convert.ToString(row["Email"]);
                c.Address = Convert.ToString(row["Address"]);
                c.RegionID = Convert.ToInt32(row["RegionID"]);
                list.Add(c);
            }
            return list;
        }
    }
}
