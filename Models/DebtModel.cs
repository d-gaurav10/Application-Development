using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackfy.Models
{
    public class DebtModel
    {
        public DebtModel(string DebtId, string TransactionType, string DebtTitle, DateTime DebtDate,
                      string DebtSource, float DebtAmount, string TransactionFlow,
                      string DebtStatus, string TransactionStatus, string Username, string debtNote)
        {
            this.DebtId = DebtId;
            this.TransactionType = TransactionType;
            this.DebtTitle = DebtTitle;
            this.DebtDate = DebtDate;
            this.DebtSource = DebtSource;
            this.DebtAmount = DebtAmount;
            this.TransactionFlow = TransactionFlow;
            this.DebtStatus = DebtStatus;
            this.TransactionStatus = TransactionStatus;
            this.Username = Username;
            this.DebtNote = debtNote;
        }

        // Unique ID for the debt record
        public string DebtId { get; set; }

        // Transaction type (e.g., "Loan", "Credit")
        public string TransactionType { get; set; }

        // Title or description of the debt
        public string DebtTitle { get; set; }

        public string DebtNote { get; set; }

        // Date of the debt transaction
        public DateTime DebtDate { get; set; }

        // Source of the debt (e.g., "Bank", "Friend")
        public string DebtSource { get; set; }

        // Amount of the debt
        public float DebtAmount { get; set; }

        // Transaction flow (e.g., "Incoming", "Outgoing")
        public string TransactionFlow { get; set; }

        // Status of the debt (e.g., "Pending", "Paid", "Overdue")
        public string DebtStatus { get; set; }

        // Transaction status (e.g., "Success", "Failed")
        public string TransactionStatus { get; set; }

        // Associated username (foreign key reference to the User model)
        public string Username { get; set; }
    }
}
