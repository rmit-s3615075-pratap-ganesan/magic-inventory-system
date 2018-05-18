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
    [Authorize(Roles = "Retail")]
    public class FranchiseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
      
        public FranchiseController(UserManager<ApplicationUser> userManager, ApplicationDbContext context )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Franchise
        public async Task<IActionResult> CreateNewStock(string searchString)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.myData = _context.Stores.Where(x => x.StoreID == user.StoreID).Select(x => x.Name).First();
            var query = _context.StoreInventory
                        .Include(x => x.Product)
                        .Select(x => x)
                        .Where(x => x.StoreID == user.StoreID);
            
            if ( searchString != null )
            {
                 query = _context.StoreInventory
                   .Include(x => x.Product)
                   .Select(x => x)
                                 .Where(x => x.StoreID == user.StoreID).Where(x=> x.Product.Name.Contains(searchString));
            }
            ViewBag.SearchString = searchString;
           
            return View(await query.ToListAsync());
        }
      
        public async Task<IActionResult> NewInventory()
        {

            var model = new List<OwnerInventory>();
            var user = await _userManager.GetUserAsync(User);
            ViewBag.myData = _context.Stores.Where(x => x.StoreID == user.StoreID).Select(x => x.Name).First();

            var storeProduct = _context.StoreInventory.Where(x => x.StoreID == user.StoreID).Select(x => x.Product).ToList();
            var newItems = _context.OwnerInventory.Select(x => x.Product).ToList().Except(storeProduct).ToList();
            if ( newItems.Count() == 0 )
            {
                ViewBag.content = "There are no request right now. Come back later";
            }

            return View(newItems);
        }

        // GET: Franchise/EditNewRequest/5
        public async Task<IActionResult> NewRequest(int? id)
        {

            var user = await _userManager.GetUserAsync(User);

            ViewBag.myData = _context.Stores.Where(x => x.StoreID == user.StoreID).Select(x => x.Name).First();

            if (id == null)
            {
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewRequest([Bind("StoreID,ProductID,StockLevel")] StoreInventory storeInventory , Product Product )
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
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(CreateNewStock));
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", storeInventory.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Stores, "StoreID", "StoreID", storeInventory.StoreID);
            return View(storeInventory);
        }

     

        // GET: Franchise/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var storeInventory = await _context.StoreInventory
                                               .Include(s => s.Product).Where( s => s.ProductID == id)
                                               .Include(s => s.Store)
                                               .SingleOrDefaultAsync(m => m.StoreID == user.StoreID);
            if (storeInventory == null)
            {
                return NotFound();
            }

            return View(storeInventory);
        }

     

        // GET: Franchise/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var storeInventory =  _context.StoreInventory
                                               .Include(x => x.Product)
                                               .Where(x => x.ProductID.Equals(id))
                                               .Select(x => x)
                                               .Where(x => x.Store.StoreID == user.StoreID).FirstOrDefault();


            if (storeInventory == null)
            {
                return NotFound();
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", storeInventory.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Stores, "StoreID", "StoreID", storeInventory.StoreID);
            return View(storeInventory);
        }

        // POST: Franchise/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( [Bind("StoreID,ProductID,StockLevel")] StoreInventory storeInventory)
        {
            var user = await _userManager.GetUserAsync(User);
            var stockLevel = storeInventory.StockLevel;
            var productID = storeInventory.ProductID;
            var storeID = storeInventory.StoreID;

            if ( user.StoreID != storeInventory.StoreID )
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
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", storeInventory.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Stores, "StoreID", "StoreID", storeInventory.StoreID);
            return View(storeInventory);
        }

        // GET: Franchise/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var storeInventory = await _context.StoreInventory
                .Include(s => s.Product)
                .Include(s => s.Store)
                .SingleOrDefaultAsync(m => m.StoreID == id);
            if (storeInventory == null)
            {
                return NotFound();
            }

            return View(storeInventory);
        }

        // POST: Franchise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var storeInventory = await _context.StoreInventory.SingleOrDefaultAsync(m => m.StoreID == id);
            _context.StoreInventory.Remove(storeInventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(CreateNewStock));
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
