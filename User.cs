using System;

namespace AIOSICT011.Models
{
    // User account (the password hash is never kept in the object)
    public class User
    {
        public const string Admin = "Admin";
        public const string CustomerRole = "Customer";
        public const string ProviderRole = "Provider";

        public int UserID { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }      // Admin, Customer, Provider
        public int PersonID { get; set; }     // CustomerID or ProviderID (0 for Admin)
    }

    // Static session helper
    public static class CurrentSession
    {
        public static User LoggedInUser { get; set; }
        public static bool LoggedOut { get; set; }

        public static bool IsAdmin { get { return LoggedInUser != null && LoggedInUser.Role == User.Admin; } }
        public static bool IsCustomer { get { return LoggedInUser != null && LoggedInUser.Role == User.CustomerRole; } }
        public static bool IsProvider { get { return LoggedInUser != null && LoggedInUser.Role == User.ProviderRole; } }
    }
}
