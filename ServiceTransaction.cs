using System;

namespace AIOSICT011.Models
{
    // Status values used by the workflow:
    // Requested -> In Progress -> Completed (invoice issued) -> Paid (customer paid AIOS) -> Closed (AIOS paid provider)
    public static class TransactionStatus
    {
        public const string Requested = "Requested";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string Paid = "Paid";
        public const string Closed = "Closed";
    }

    // Model for booking / job transactions
    public class ServiceTransaction : IReportable
    {
        public int TransactionID { get; set; }
        public int CustomerID { get; set; }
        public int ProviderID { get; set; }
        public DateTime ServiceDate { get; set; }
        public decimal HoursWorked { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string InvoiceNo { get; set; }
        public bool CustomerPaid { get; set; }
        public bool ProviderPaid { get; set; }
        public int Rating { get; set; }
        public string Feedback { get; set; }
        public bool RedFlag { get; set; }

        public string GetSummaryText()
        {
            return "Job #" + TransactionID + " | Date: " + ServiceDate.ToShortDateString() + " | Status: " + Status + " | Total: P" + Amount;
        }
    }
}
