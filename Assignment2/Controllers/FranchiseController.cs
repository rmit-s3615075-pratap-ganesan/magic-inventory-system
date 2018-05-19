/**
 * @author: Niraj Bohra 
 * @version 1.0
 */

// imports
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Assignment2.Controllers
{
    // Allowed role based users
    [Authorize(Roles = "Retail")]
    public class FranchiseController : Controller
    {
        // Db context 
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public FranchiseController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Allows franchise to view stock and filter product.
        public async Task<IActionResult> CreateNewStock(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.myData = _context.Stores.Where(x => x.StoreID == user.StoreID).Select(x => x.Name).First();
            var query = _context.StoreInventory
                        .Include(x => x.Product)
                        .Select(x => x)
                        .Where(x => x.StoreID == user.StoreID);
            // Return string specific data 
            if (searchString != null)
            {
                query = _context.StoreInventory
                  .Include(x => x.Product)
                  .Select(x => x)
                                .Where(x => x.StoreID == user.StoreID).Where(x => x.Product.Name.Contains(searchString));
            }
            ViewBag.SearchString = searchString;

            return View(await query.ToListAsync());
        }
        // Allows franchise to add new inventory if any.
        public async Task<IActionResult> NewInventory()
        {

            var model = new List<OwnerInventory>();
            var user = await _userManager.GetUserAsync(User);
            ViewBag.myData = _context.Stores.Where(x => x.StoreID == user.StoreID).Select(x => x.Name).First();

            var storeProduct = _context.StoreInventory.Where(x => x.StoreID == user.StoreID).Select(x => x.Product).ToList();
            var newItems = _context.OwnerInventory.Select(x => x.Product).ToList().Except(storeProduct).ToList();
            if (newItems.Count() == 0)
            {
                ViewBag.content = "There are no request right now. Come back later";
            }

            return View(newItems);
        }

        // GET: Create stock request for both new and exsisting  
        public async Task<IActionResult> NewRequest(int? id)
        {

            var user = await _userManager.GetUserAsync(User);

            ViewBag.myData = _context.Stores.Where(x => x.StoreID == user.StoreID).Select(x => x.Name).First();

            if (id == null)
            {
                // throw not found errors
                return NotFound();

            }

            var storeInventory = new StoreInventory();
            storeInventory.StoreID = user.StoreID ?? 0;
            storeInventory.Product = _context.Products.Where(x => x.ProductID == id).First();


            if (storeInventory == null)
            {
                return NotFound();
            }

            return View(storeInventory);
        }
        // Secure post submit data POST method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewRequest([Bind("StoreID,ProductID,StockLevel")] StoreInventory storeInventory, Product Product)
        {
            var user = await _userManager.GetUserAsync(User);
            var stockLevel = storeInventory.StockLevel;
            var productID = storeInventory.ProductID;
            var storeID = storeInventory.StoreID;

            if (user.StoreID != storeInventory.StoreID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create stock request and save to context
                    StockRequest stockReq = new StockRequest();
                    stockReq.ProductID = Product.ProductID;
                    stockReq.Quantity = stockLevel;
                    stockReq.StoreID = storeID;
                    _context.StockRequest.Add(stockReq);


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreInventoryExists(storeInventory.StoreID))
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(CreateNewStock));
            }
  
            return View(storeInventory);
        }

      
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var storeInventory = _context.StoreInventory
                                               .Include(x => x.Product)
                                               .Where(x => x.ProductID.Equals(id))
                                               .Select(x => x)
                                               .Where(x => x.Store.StoreID == user.StoreID).FirstOrDefault();


            return View(storeInventory);
        }

        // POST: Franchise/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("StoreID,ProductID,StockLevel")] StoreInventory storeInventory)
        {
            var user = await _userManager.GetUserAsync(User);
            var stockLevel = storeInventory.StockLevel;
            var productID = storeInventory.ProductID;
            var storeID = storeInventory.StoreID;

            if (user.StoreID != storeInventory.StoreID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    StockRequest stockReq = new StockRequest();
                    stockReq.ProductID = productID;
                    stockReq.Quantity = stockLevel;
                    stockReq.StoreID = storeID;
                    _context.StockRequest.Add(stockReq);


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StoreInventoryExists(storeInventory.StoreID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CreateNewStock));
            }

            return View(storeInventory);
        }

        private bool StoreInventoryExists(int id)
        {
            return _context.StoreInventory.Any(e => e.StoreID == id);
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}