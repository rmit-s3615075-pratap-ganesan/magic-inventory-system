using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Models;
using Assignment2.Models.CartViewModels;
using Assignment2.Utility;
using Assignment2.Data;
using Microsoft.AspNetCore.Http;

namespace Assignment2.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> Index(
            string sortOrder, string currentFilter,
            string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["StoreSortParm"] = sortOrder == "Store" ? "store_desc" : "Store";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;



            var query = _context.StoreInventory
                                .Include(x => x.Product)
                                .Include(x=>x.Store)
                                .Select(x => x);
            //var storeID = _context.Store.Where(x=>x.Name.Contains("bourne")).Select(x=>x.StoreID);
                                

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(x => x.Product.Name.Contains(searchString));

            }

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(s => s.Product.Name);
                    break;
                case "Store":
                    query = query.OrderBy(s => s.Store.Name);
                    break;
                case "store_desc":
                    query = query.OrderByDescending(s => s.Store.Name);
                    break;
                default:
                    query = query.OrderBy(s => s.Product.Name);
                    break;
            }

            int pageSize = 3;

            return View(await PaginatedList<StoreInventory>
                        .CreateAsync(query.AsNoTracking(), page ?? 1, pageSize));
        }


        public async Task<IActionResult> Buy(int? storeid,int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query =  _context.StoreInventory
                                .Include(x => x.Product)
                                .Include(x => x.Store)
                                 .Where(x => x.StoreID == storeid)
                                .Where(x => x.ProductID == id).First<StoreInventory>();
                                 
            if (query == null)
            {
                return NotFound();
            }
            //else{
            //    new CartViewModel
            //    {
            //        Product = query.Product,
            //        Quantity = query.ProductID

            //    };
            //}   
            return View(query);
        }


        [HttpPost, ActionName("Buy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(int? stockLevel)
        {
            if (stockLevel == null)
            {
                return NotFound();
            }

            CartViewModel cart = new CartViewModel();
            cart.ProductID = Convert.ToInt32("" + Request.Form["ProductID"]);
            cart.ProductName = ""+Request.Form["Product.Name"];
            cart.StoreID = Convert.ToInt32("" + Request.Form["StoreID"]);
            cart.StoreName = "" + Request.Form["Store.Name"];
            cart.Price = Convert.ToDecimal(""+Request.Form["Product.Price"]);
            cart.Quantity = stockLevel ?? 0;

            ShoppingList.AddToShoppingList(cart);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Cart(){
            
            CartViewModel cart = new CartViewModel();
            foreach (var Kart in ShoppingList.GetAllShoppingList().Keys)
                cart = ShoppingList.GetAllShoppingList()[Kart];

            List<CartViewModel> cartList = ShoppingList.GetAllShoppingList().Values.ToList<CartViewModel>();

            return View(cartList);
        }
         
    }
}
