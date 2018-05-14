using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Models;
using Assignment2.Models.CartViewModels;
using Assignment2.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Assignment2.Controllers
{
    public class OrderHistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderHistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Create(){
            CustomerOrder newOrder = new CustomerOrder();
            newOrder.UserEmail = "pratap1288@gmail.com";
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
            foreach(CartViewModel cart in shoppingList){
                newOrderHistory = new OrderHistory();
                newOrderHistory.ReceiptID = receiptID;
                newOrderHistory.ProductName = cart.ProductName;
                newOrderHistory.StoreName = cart.StoreName;
                newOrderHistory.Quantity = cart.Quantity;
                newOrderHistory.TotalPrice = cart.TotalPrice;
                _context.Add(newOrderHistory);

               // var storeContext = _context.StoreInventory.Where(x => x.ProductID == productID).Select(x => x).First();
               // storeContext.StockLevel += stockRequestToUpdate.Quantity;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));

        }
    }
}

