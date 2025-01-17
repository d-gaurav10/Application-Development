using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trackfy.Models
{
    public class TransactionModel
    {

        public TransactionModel(string trId, string trType, string trTitle, string trFlow, DateTime trDate,
                                string trSource, string trNote, float trAmount, string userUsername)
        {
            TrId = trId;
            TrType = trType;
            TrTitle = trTitle;
            TrFlow = trFlow;
            TrDate = trDate;
            TrSource = trSource;
            TrNote = trNote;
            TrAmount = trAmount;
            UserUsername = userUsername;
        }

        public string TrId { get; set; }           
        public string TrType { get; set; }      
        public string TrTitle { get; set; }      
        public string TrFlow { get; set; }       
        public DateTime TrDate { get; set; }    
        public string TrSource { get; set; }      
        public string TrNote { get; set; }     
        public float TrAmount { get; set; }   
        public string UserUsername { get; set; }

    }
}
