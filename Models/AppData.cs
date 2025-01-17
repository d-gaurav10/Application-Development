using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackfy.Models
{
    public class AppData
    {
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();
        public List<DebtModel> Debts { get; set; } = new List<DebtModel>();
    }

}
