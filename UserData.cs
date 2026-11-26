using System;
using System.Data;
using System.Data.SqlClient;
using AIOSICT011.Models;

namespace AIOSICT011.DataAccess
{
    public static class UserData
    {
        // Returns the user when username and password hash match, otherwise null
        public static User Find(string username, string passwordHash)
        {
            DataTable dt = Db.GetData(
                "SELECT UserID, Username, Role, PersonID FROM Users WHERE Username = @u AND PasswordHash = @h",
                new SqlParameter("@u", username),
                new SqlParameter("@h", passwordHash));

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];
            User user = new User();
            user.UserID = Convert.ToInt32(row["UserID"]);
            user.Username = Convert.ToString(row["Username"]);
            user.Role = Convert.ToString(row["Role"]);
            if (row["PersonID"] == DBNull.Value)
                user.PersonID = 0;
            else
                user.PersonID = Convert.ToInt32(row["PersonID"]);
            return user;
        }

        public static bool UsernameExists(string username)
        {
            object count = Db.GetValue("SELECT COUNT(*) FROM Users WHERE Username = @u",
                new SqlParameter("@u", username));
            return Convert.ToInt32(count) > 0;
        }

        public static void Insert(string username, string passwordHash, string role, int personId)
        {
            Db.Execute(
                "INSERT INTO Users (Username, PasswordHash, Role, PersonID) VALUES (@u, @h, @r, @p)",
                new SqlParameter("@u", username),
                new SqlParameter("@h", passwordHash),
                new SqlParameter("@r", role),
                new SqlParameter("@p", personId));
        }
    }
}
