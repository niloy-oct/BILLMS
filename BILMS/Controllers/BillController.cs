using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BILMS.Code;
using BILMS.Models;
using BILMS.ViewModels;

namespace BILMS.Controllers
{
    public class BillController : Controller
    {
        private DatabaseContext _dbContext;

        public BillController(DatabaseContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public ActionResult Index()
        {

            List<Inventory> billlist = _dbContext.Inventories.ToList();
            return View(billlist);
        }
        public ActionResult Create()
        {
            BillEntryViewModel billEntryViewModel = (BillEntryViewModel)TempData["BillEntryViewModel"];
            if (billEntryViewModel == null)
            {
                GetViewBagData();
                return View(new BillEntryViewModel() { Details = new List<BillEntryDetailVM>() });
            }
            else
            {

                TempData["BillEntryViewModel"] = billEntryViewModel;
                GetViewBagData();
                ViewBag.SelectedCustomers = billEntryViewModel.CustomerId;
                ViewBag.SelectedProducts = billEntryViewModel.Detail.ProductId;
                return View(billEntryViewModel);
            }
        }

        [HttpPost]
        public ActionResult Create(BillEntryViewModel newbillEntryViewModel, FormCollection formcollection)
        {
            return HandleBillEntry(newbillEntryViewModel, formcollection);
        }

        private ActionResult HandleBillEntry(BillEntryViewModel newbillEntryViewModel, FormCollection formcollection)
        {
            BillEntryViewModel billEntryViewModel = (BillEntryViewModel)TempData.Peek("BillEntryViewModel");
            billEntryViewModel = billEntryViewModel ?? new BillEntryViewModel()
            {
                Details = new List<BillEntryDetailVM>()
            };

            if (formcollection.Get("btnAdd") != null)
            {
                AddToOrderError(newbillEntryViewModel, billEntryViewModel, formcollection);
                bool productExists = DuplicateProductEntry(billEntryViewModel, formcollection["ProductsList"]);

                if (productExists)
                {
                    ModelState.AddModelError(string.Empty, "");
                    TempData["AlertMessage"] = " this product already added to bill items, please select other product";
                    TempData["AlertType"] = "danger";
                }
                if (!ModelState.IsValid)
                {
                    ViewBag.SelectedCustomers = formcollection["CustomersList"];
                    return View("Create", billEntryViewModel);
                }
                AddToOrder(newbillEntryViewModel, billEntryViewModel, formcollection);
                ModelState.Clear();
                ViewBag.SelectedCustomers = billEntryViewModel.CustomerId;
                GetViewBagData();
                TempData["AlertMessage"] = "Bill item add successfully.";
                TempData["AlertType"] = "success";
                return View("Create", billEntryViewModel);
            }
            else if (formcollection.Get("btnSave") != null)
            {
                SaveBillEntry(newbillEntryViewModel, billEntryViewModel, formcollection);
                TempData.Clear();
                TempData["AlertMessage"] = "Bill entry save successfully.";
                TempData["AlertType"] = "success";
            }
            else if (formcollection.Get("btnEdit") != null)
            {

                UpdateBillEntry(newbillEntryViewModel, billEntryViewModel, formcollection);
                TempData.Clear();
                TempData["AlertMessage"] = "Bill entry update successfully.";
                TempData["AlertType"] = "success";

            }
            return RedirectToAction("Index");
        }
        private void AddToOrderError(BillEntryViewModel newEntryViewModel, BillEntryViewModel billEntryViewModel, FormCollection formcollection)
        {
            string productId = formcollection["ProductsList"];
            if (string.IsNullOrEmpty(productId))
            {
                ModelState.AddModelError("", "");
                TempData["AlertMessage"] = "please select product";
                TempData["AlertType"] = "danger";
            }

            GetViewBagData();
        }
        
        private void AddToOrder(BillEntryViewModel newbillEntryViewModel, BillEntryViewModel billEntryViewModel, FormCollection formcollection)
        {
            string customerId = formcollection["CustomersList"];
            string productId = formcollection["ProductsList"];


            BillEntryDetailVM newDetail = new BillEntryDetailVM();
            newDetail.ProductId = productId;
            int ProductID = Convert.ToInt32(productId);
            newDetail.ProductName =
                _dbContext.Product.Where(i => i.Id == ProductID).Select(x => x.Name).FirstOrDefault();
            newDetail.Rate = newbillEntryViewModel.Detail.Rate;
            newDetail.Qty = newbillEntryViewModel.Detail.Qty;
            newDetail.TotalAmount = newbillEntryViewModel.Detail.TotalAmount;
            newDetail.Discount = newbillEntryViewModel.Detail.Discount;
            newDetail.NetAmount = newbillEntryViewModel.Detail.NetAmount;

            billEntryViewModel.Details.Add(newDetail);

            billEntryViewModel.BillNo = newbillEntryViewModel.BillNo;
            billEntryViewModel.BillDate = newbillEntryViewModel.BillDate;
            billEntryViewModel.CustomerId = customerId;
            billEntryViewModel.NetAmt = billEntryViewModel.Details.Sum(i => i.NetAmount);
            billEntryViewModel.TotalDis = billEntryViewModel.Details.Sum(i => i.Discount);
            TempData["BillEntryViewModel"] = billEntryViewModel;

        }

        private bool DuplicateProductEntry(BillEntryViewModel billEntryViewModel, string productId)
        {
            return billEntryViewModel.Details.Any(detail => detail.ProductId == productId);
        }
        public JsonResult GetProductInfo(string productId)
        {
            int productID = Convert.ToInt32(productId);
            var product = _dbContext.Product
                .Where(i => i.Id == productID)
                .Select(x => new { Name = x.Name, Rate = x.Rate })
                .FirstOrDefault();

            if (product != null)
            {
                decimal rate = product.Rate;
                decimal quantity = 1;
                decimal totalAmount = quantity * rate;
                decimal discountAmount = 0;
                decimal netAmount = totalAmount - discountAmount;

                var productInfo = new
                {
                    Rate = rate,
                    Qty = quantity,
                    TotalAmount = totalAmount,
                    DiscountAmount = discountAmount,
                    NetAmount = netAmount
                };

                return Json(productInfo, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DeleteFromView(string ProductId, string previousAction)
        {
            BillEntryViewModel billEntryViewModel = (BillEntryViewModel)TempData.Peek("BillEntryViewModel");
            var itemToDelete = billEntryViewModel.Details.Where(x => x.ProductId == ProductId).FirstOrDefault();
            billEntryViewModel.Details.Remove(itemToDelete);

            GetViewBagData();
            ViewBag.SelectedCustomers = billEntryViewModel.CustomerId;
            billEntryViewModel.NetAmt = billEntryViewModel.NetAmt - itemToDelete.NetAmount;
            billEntryViewModel.TotalDis = billEntryViewModel.TotalDis - itemToDelete.Discount;
            TempData["BillEntryViewModel"] = billEntryViewModel;
            billEntryViewModel.Detail = new BillEntryDetailVM();
            return RedirectToAction("Create");
        }

        [HttpGet]
        public ActionResult EditFromView(string ProductId, string previousAction)
        {
            BillEntryViewModel billEntryViewModel = (BillEntryViewModel)TempData.Peek("BillEntryViewModel");
            var itemToEdit = billEntryViewModel.Details.Where(x => x.ProductId == ProductId).FirstOrDefault();
            billEntryViewModel.Details.Remove(itemToEdit);
            billEntryViewModel.Detail = new BillEntryDetailVM();

            GetViewBagData();
            
            ViewBag.SelectedCustomers = billEntryViewModel.CustomerId;
            billEntryViewModel.Detail.ProductId = ProductId;
            billEntryViewModel.Detail.Rate = itemToEdit.Rate;
            billEntryViewModel.Detail.Qty = itemToEdit.Qty;
            billEntryViewModel.Detail.TotalAmount = itemToEdit.TotalAmount;
            billEntryViewModel.Detail.Discount = itemToEdit.Discount;
            billEntryViewModel.Detail.NetAmount = itemToEdit.NetAmount;
            TempData["BillEntryViewModel"] = billEntryViewModel;
            return RedirectToAction("Create");
        }

        [HttpGet]
        public ActionResult Edit(string BillNo)
        {
            var VM = new BillEntryViewModel();
            VM.Detail = new BillEntryDetailVM();
            VM.Details = new List<BillEntryDetailVM>();
            var inventories = _dbContext.Inventories
                .Where(i => i.BillNo == BillNo)
                .FirstOrDefault();
            var inventoriesProduct = _dbContext.InventoryProducts
                .Where(i => i.InventoryId == inventories.Id).ToList();
            foreach (var iteam in inventoriesProduct)
            {
                var detail = new BillEntryDetailVM()
                {
                    ProductId = iteam.ProductId.ToString(),
                    ProductName = _dbContext.Product.Where(i => i.Id == iteam.ProductId).Select(x => x.Name).FirstOrDefault(),
                    Rate = iteam.Rate,
                    Qty = Convert.ToDecimal(iteam.Qty),
                    Discount = iteam.Discount,
                    TotalAmount = Convert.ToDecimal(iteam.Rate * iteam.Qty),
                    NetAmount = Convert.ToDecimal((iteam.Rate * iteam.Qty) - iteam.Discount)

                };
                VM.Details.Add(detail);
            }

            VM.BillNo = inventories.BillNo;
            VM.BillDate = inventories.Date.ToString("dd/MM/yyyy");
            VM.CustomerId = inventories.CustomerId.ToString();
            VM.NetAmt = inventories.TotalBillAmount;
            VM.PaidAmt = inventories.PaidAmount;
            VM.TotalDis = inventories.TotalDiscount;
            VM.DueAmt = inventories.DueAmount;
            VM.InventoryId = inventories.Id.ToString();
            GetViewBagData();
            ViewBag.SelectedCustomers = VM.CustomerId;
            TempData["BillEntryViewModel"] = VM;

            return View(VM);
        }

        [HttpPost]
        public ActionResult Edit(BillEntryViewModel newbillEntryViewModel, FormCollection formcollection)
        {
            return HandleBillEntry(newbillEntryViewModel, formcollection);
        }

        private void SaveBillEntry(BillEntryViewModel newbillEntryViewModel, BillEntryViewModel billEntryViewModel, FormCollection formcollection)
        {
            try
            {
                var inventory = new Inventory();
                inventory.Date = DateTime.Parse(newbillEntryViewModel.BillDate);
                inventory.BillNo = newbillEntryViewModel.BillNo;
                inventory.CustomerId = Convert.ToInt32(billEntryViewModel.CustomerId);
                inventory.TotalDiscount = Convert.ToDecimal(newbillEntryViewModel.TotalDis);
                inventory.TotalBillAmount = Convert.ToDecimal(newbillEntryViewModel.NetAmt);
                inventory.DueAmount = Convert.ToDecimal(newbillEntryViewModel.DueAmt);
                inventory.PaidAmount = Convert.ToDecimal(newbillEntryViewModel.PaidAmt);
                _dbContext.Inventories.Add(inventory);
                _dbContext.SaveChanges();

                // Access the generated Id after saving changes to the database
                int inventoryId = inventory.Id;

                foreach (var item in billEntryViewModel.Details)
                {
                    var inventoryProduct = new InventoryProduct();
                    inventoryProduct.InventoryId = inventoryId; // Set the InventoryId to the newly created inventory's Id
                    inventoryProduct.ProductId = Convert.ToInt32(item.ProductId);
                    inventoryProduct.Rate = item.Rate;
                    inventoryProduct.Qty = Convert.ToInt32(item.Qty);
                    inventoryProduct.Discount = item.Discount;
                    _dbContext.InventoryProducts.Add(inventoryProduct);
                }

                _dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }

            GetViewBagData();
        }

        private void UpdateBillEntry(BillEntryViewModel newbillEntryViewModel, BillEntryViewModel billEntryViewModel, FormCollection formcollection)
        {
            try
            {
                int inventoryId = Convert.ToInt32(billEntryViewModel.InventoryId);
                // Retrieve the existing inventory record to update
                var existingInventory = _dbContext.Inventories.FirstOrDefault(i => i.Id == inventoryId);

                if (existingInventory != null)
                {
                    // Update the properties of the existing inventory record
                    existingInventory.Date = DateTime.Parse(newbillEntryViewModel.BillDate);
                    existingInventory.BillNo = newbillEntryViewModel.BillNo;
                    existingInventory.CustomerId = Convert.ToInt32(formcollection["CustomersList"]);
                    existingInventory.TotalDiscount = Convert.ToDecimal(newbillEntryViewModel.TotalDis);
                    existingInventory.TotalBillAmount = Convert.ToDecimal(newbillEntryViewModel.NetAmt);
                    existingInventory.DueAmount = Convert.ToDecimal(newbillEntryViewModel.DueAmt);
                    existingInventory.PaidAmount = Convert.ToDecimal(newbillEntryViewModel.PaidAmt);


                    // Update the existing inventory products
                    foreach (var item in billEntryViewModel.Details)
                    {
                        int productId = Convert.ToInt32(item.ProductId);

                        var existingInventoryProduct = _dbContext.InventoryProducts.FirstOrDefault(ip => ip.InventoryId == inventoryId && ip.ProductId == productId);

                        if (existingInventoryProduct != null)
                        {
                            // Update the properties of the existing inventory product
                            existingInventoryProduct.Rate = item.Rate;
                            existingInventoryProduct.Qty = Convert.ToInt32(item.Qty);
                            existingInventoryProduct.Discount = item.Discount;
                        }
                    }

                    _dbContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            GetViewBagData();
        }

        private void GetViewBagData()
        {
            var customers = _dbContext.Customer.ToList();
            var customerList = customers.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            ViewBag.Customers = customerList;

            var products = _dbContext.Product.ToList();
            var productList = products.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            }).ToList();

            ViewBag.Products = productList;
        }


    }
}