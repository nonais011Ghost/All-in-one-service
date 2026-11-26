using System;

namespace AIOSICT011.Models
{
    // Abstract base class demonstrating encapsulation and inheritance
    public abstract class Person
    {
        // Private encapsulated fields
        private string fullName;
        private string phone;

        // Public properties with simple validation
        public string FullName
        {
            get { return fullName; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Full name cannot be empty.");
                fullName = value.Trim();
            }
        }

        public string Phone
        {
            get { return phone; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Phone number cannot be empty.");
                phone = value.Trim();
            }
        }

        public string Address { get; set; }
        public int RegionID { get; set; }

        // Constructors (Lesson 5): an empty one for loading from the database,
        // and one that sets the common details in a single call.
        protected Person() { }

        protected Person(string fullName, string phone, string address, int regionID)
        {
            FullName = fullName;
            Phone = phone;
            Address = address;
            RegionID = regionID;
        }

        // Abstract method (must be overridden by child classes)
        public abstract string GetRole();

        // Virtual method (polymorphism)
        public virtual string DisplayInfo()
        {
            return FullName + " (" + Phone + ") - " + GetRole();
        }
    }
}
