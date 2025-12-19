using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DigitalBankLite.API.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Simplified for "Lite" version
        public string KycStatus { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string Role { get; set; } = "Customer"; // Customer, Admin
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public List<Account>? Accounts { get; set; }
        [JsonIgnore]
        public List<ServiceRequest>? ServiceRequests { get; set; }
    }

    public class Account
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = "Savings"; // Savings, Current
        public decimal Balance { get; set; }
        public string Status { get; set; } = "Active"; // Active, Inactive
        
        [JsonIgnore]
        public List<Transaction>? Transactions { get; set; }
    }

    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public Account? Account { get; set; }
        public DateTime TxnDateTime { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = "Credit"; // Credit, Debit
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal BalanceAfterTxn { get; set; }
    }

    public class ServiceRequest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open"; // Open, In Progress, Closed
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }
    }
}
