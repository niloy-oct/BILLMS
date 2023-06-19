using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BILMS.Models
{
   
    [Table("Inventories",Schema = "dbo")]
    public class Inventory
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string BillNo { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalBillAmount { get; set; }
        public decimal DueAmount { get; set; }
        public decimal PaidAmount { get; set; }

        public Customer Customer { get; set; }
    }


}