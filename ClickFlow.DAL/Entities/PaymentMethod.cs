using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickFlow.DAL.Entities
{
    public class PaymentMethod
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }  
        public string PaymentInfo { get; set; }
        public string? BankName { get; set; } 
        public bool IsDefault { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
