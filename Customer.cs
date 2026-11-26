using System;

namespace AIOSICT011.Models
{
    // Subclass inheriting from Person and implementing IReportable
    public class Customer : Person, IReportable
    {
        public int CustomerID { get; set; }
        public string MembershipID { get; set; }
        public string Email { get; set; }

        public Customer() { }

        public Customer(string fullName, string phone, string address, int regionID, string email)
            : base(fullName, phone, address, regionID)
        {
            Email = email;
        }

        public override string GetRole()
        {
            return "Customer";
        }

        public string GetSummaryText()
        {
            return "Customer #" + CustomerID + ": " + FullName + " | Contact: " + Phone;
        }
    }
}
