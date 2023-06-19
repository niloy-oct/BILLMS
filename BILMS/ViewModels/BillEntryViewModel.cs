using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BILMS.ViewModels
{
    public class BillEntryViewModel
    {
        public BillEntryViewModel()
        {
            Details = new List<BillEntryDetailVM>();

        }
        public string InventoryId { get; set; }

        [Display(Name = "Bill No..")]
        public string BillNo { get; set; }

        [Display(Name = "Date")]
        public string BillDate { get; set; }

        [Display(Name = "Customer")]
        public string CustomerId { get; set; }

        [Display(Name = "Product")]
        public string ProductId { get; set; }

        [Display(Name = "Total Discount")]
        public decimal TotalDis { get; set; }

        [Display(Name = "Due Amount")]
        public decimal DueAmt { get; set; }
        [Display(Name = "Paid Amount")]
        public decimal PaidAmt { get; set; }

        [Display(Name = "Net Amount")]
        public decimal NetAmt { get; set; }

        public BillEntryDetailVM Detail { get; set; }
        public List<BillEntryDetailVM> Details { get; set; }
    }

    public class BillEntryDetailVM
    {
        public string InventoryProductId { get; set; }
        public string InventoryId { get; set; }
        public string ProductId { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "Product Rate")]
        public decimal Rate { get; set; }

        [Display(Name = "Product Qty")]
        public decimal Qty { get; set; }
        [Display(Name = "Product Discount")]
        public decimal Discount { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
    }
}