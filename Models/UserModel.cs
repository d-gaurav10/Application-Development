using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Trackfy.Models
{
    public class UserModel
    {
        public UserModel(
             string user_username,                              
             string user_userPassword,
             float user_DebtBalance,
             float user_AvailableBalance,
             string user_firstName,
             string user_lastName,
            string user_type)
        {
            Username = user_username;
            UserPassword = user_userPassword;
            UserDebtBalance = user_DebtBalance;
            UserAvailableBalance = user_AvailableBalance;
            FirstName = user_firstName;
            LastName = user_lastName;
            UserType = user_type;
        }
        // Username for the user
        [Required]
        public string Username { get; set; } 

        // User password
        [Required]
        public string UserPassword { get; set; }

        // Debt balance: the amount the user owes
        [Required]
        public float UserDebtBalance { get; set; }

        // Available balance: the amount the user has available
        [Required]
        public float UserAvailableBalance { get; set; }

        // User's first name
        [Required]
        public string FirstName { get; set; }

        // User's last name
        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserType { get; set; }

        public string CurrencyType { get; set; }
    }
}
