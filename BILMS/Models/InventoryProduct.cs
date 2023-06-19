using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BILMS.Models
{
    [Table("InventoryProducts", Schema = "dbo")]
    public class InventoryProduct
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Inventory")]
        public int InventoryId { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public decimal Rate { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }

        public Inventory Inventory { get; set; }
        public Product Product { get; set; }
    }


}