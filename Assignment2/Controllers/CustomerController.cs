﻿using System;
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
using Microsoft.AspNetCore.Authorization;

namespace Assignment2.Controllers
{
    [Authorize(Roles ="Customer")]
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
                                .Include(x => x.Store)
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


        public async Task<IActionResult> Buy(int? storeid, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.StoreInventory
                                .Include(x => x.Product)
                                .Include(x => x.Store)
                                 .Where(x => x.StoreID == storeid)
                                .Where(x => x.ProductID == id).FirstAsync<StoreInventory>();

            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }


        [HttpPost, ActionName("Buy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyPost(StoreInventory storeInventory)
        {
            if (storeInventory.StockLevel == 0)
            {
                return NotFound();
            }

            CartViewModel cart = new CartViewModel();
            cart.ProductID = storeInventory.ProductID;
            cart.ProductName = storeInventory.Product.Name;
            cart.StoreID = storeInventory.StoreID;
            cart.StoreName = storeInventory.Store.Name;
            cart.Price = storeInventory.Product.Price;
            cart.Quantity = storeInventory.StockLevel;
            cart.TotalPrice = cart.Quantity * cart.Price;
            //process the session key
            string cartkey = cart.ProductID + "/" + cart.StoreID;
            HttpContext.Session.Set<CartViewModel>(cartkey, cart);

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Cart()
        {
            return View(getSessionItems());
        }


        public async Task<IActionResult> DeleteCart(int prodID, int storeID)
        {

            HttpContext.Session.Remove(prodID + "/" + storeID);
            return RedirectToAction(nameof(Cart));
        }

        public async Task<IActionResult> EditCart(int prodID, int storeID)
        {
            return RedirectToAction("Action", "controller", new { @storeid = storeID, @id = prodID });
        }


        private List<CartViewModel> getSessionItems()
        {
            decimal totalPrice = 0;
            List<CartViewModel> shoppingList = new List<CartViewModel>();

            foreach (var session in HttpContext.Session.Keys)
            {
                CartViewModel cart = HttpContext.Session.Get<CartViewModel>(session);
                totalPrice += cart.TotalPrice;
                shoppingList.Add(cart);
            }
            ViewData["TotalPrice"] = totalPrice;
            return shoppingList;
        }


        public IActionResult PaymentProcessing()
        {
            return View();
        }


   }
}

