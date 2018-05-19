using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Models;
using Assignment2.Models.CartViewModels;
using Assignment2.Data;
using Microsoft.AspNetCore.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Assignment2.Controllers
{
    public class OrderHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderHistoryController(UserManager<ApplicationUser> userManager,ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Create(){
            //Save Customer order in  DB
            CustomerOrder newOrder = new CustomerOrder();
            newOrder.UserEmail = _userManager.GetUserName(User);
            newOrder.TransactionDate = DateTime.Now;
            _context.Add(newOrder);
            await _context.SaveChangesAsync();
            _context.Entry(newOrder).GetDatabaseValues();
            var receiptID = newOrder.ReceiptID;

            List<CartViewModel> shoppingList = new List<CartViewModel>();

            foreach (var session in HttpContext.Session.Keys)
            {
                CartViewModel cart = HttpContext.Session.Get<CartViewModel>(session);
                shoppingList.Add(cart);
            }

            OrderHistory newOrderHistory = new OrderHistory();
            //Save the list of Products in DB
            foreach(CartViewModel cart in shoppingList){
                newOrderHistory = new OrderHistory();
                newOrderHistory.ReceiptID = receiptID;
                newOrderHistory.ProductName = cart.ProductName;
                newOrderHistory.StoreName = cart.StoreName;
                newOrderHistory.Quantity = cart.Quantity;
                newOrderHistory.TotalPrice = cart.TotalPrice;
                _context.Add(newOrderHistory);

                var storeInventory = _context.StoreInventory.Where(x => x.Product.Name == cart.ProductName)
                                           .Where(x=> x.Store.Name == cart.StoreName).Select(x => x).First();
                storeInventory.StockLevel -= cart.Quantity;
                await _context.SaveChangesAsync();

                //Delete the session
                HttpContext.Session.Remove(storeInventory.ProductID + "/" + storeInventory.StoreID);

            }

            return RedirectToAction(nameof(CustomerOrderController.Index), "CustomerOrder",
                                    new { id = receiptID});

        }
    }
}

