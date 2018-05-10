using System;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Data;
using Assignment2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.AspNetCore.Identity;

namespace Assignment2.Controllers.Retail
{
    [Authorize(Roles = "Retail")]
    public class RetailController:Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public RetailController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Auto-parsed variables coming in from the request - there is a form on the page to send this data.
        public async Task<IActionResult> DisplayInventory(string productName)
        {
            var user = await _userManager.GetUserAsync(User);

            // Eager loading the Product table - join between OwnerInventory and the Product table.
            var query = _context.StoreInventory.Include(x => x.Product).Select(x => x).Where(x => x.StoreID == user.StoreID);

            if (!string.IsNullOrWhiteSpace(productName))
            {
                // Adding a where to the query to filter the data.
                // Note for the first request productName is null thus the where is not always added.
                query = query.Where(x => x.Product.Name.Contains(productName));
            }
           

            // Storing the search into ViewBag to populate the textbox with the same value for convenience.


            // Adding an order by to the query for the Product name.
            query = query.OrderBy(x => x.Product.ProductID);

            // Passing a List<OwnerInventory> model object to the View.
            return View(await query.ToListAsync());
        }

        // Auto-parsed variables coming in from the request - there is a form on the page to send this data.
        public async Task<IActionResult> StockRequestThreshold(int thresholdID)
        {
            var user = await _userManager.GetUserAsync(User);
            var query = _context.StoreInventory.Include(x => x.Product).Where(x => x.StockLevel < thresholdID).Select(x => x).Where(x => x.Store.StoreID == user.StoreID);

            return View(await query.ToListAsync());
        }


        public async Task<ActionResult> EditQuantity( StoreInventory storeProduct)
        {
            var user = await _userManager.GetUserAsync(User);
            Console.Write(storeProduct);
            // _context.StoreInventory.Include(x => x.Product).Select(x => x).Where(x => x.Store.StoreID.Equals(1));
           // var subQuery = _context.StoreInventory.Include(x => x.Product).Where(x => x.ProductID.Equals(id)).Select(x => x).Where(x => x.Store.StoreID == user.StoreID).FirstOrDefault();



            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult EditQuantity( int? stockUpdate )
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(movie).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(movie);
        //}

        public IActionResult Dashboard()
        {
            return View();
        }


    }
}
