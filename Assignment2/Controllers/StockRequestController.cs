using System;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Data;
using Assignment2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Assignment2.Controllers
{
    public class StockRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Owner Stock Req
        public async Task<IActionResult> DisplayRequests()
        {
            var applicationDbContext = _context.StockRequest.Include(x => x.Product).Select(x => x);
           
            if (applicationDbContext.Count() == 0 )
            {
                ViewBag.myData = "There are no request right now. Come back later";
            }
            return View(await applicationDbContext.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = await _context.StockRequest
                                .Include(x => x.Product)
                                .Include(x => x.Store)
                                 .Where(x => x.StockRequestID == id)
                               .FirstAsync();

            ViewData["stockAvailability"] = await _context.OwnerInventory
                                                   .Where(x => x.ProductID == query.ProductID)
                                                    .Select(x => x.StockLevel).FirstAsync();

            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }


        public IActionResult Process(int StockRequestID,int ProductID)
        {
            var stockRequestID = StockRequestID;

            var productID = ProductID;

            if (stockRequestID == 0)
            {
                return NotFound();
            }
            var stockRequestToUpdate = _context.StockRequest.Where(x => x.StockRequestID == stockRequestID).First();
            var ownerContext = _context.OwnerInventory.Where(x => x.ProductID == productID).Select(x => x).First();
            var storeContext = _context.StoreInventory.Where(x => x.ProductID == productID)
                                       .Where(x=>x.StoreID==stockRequestToUpdate.StoreID)
                                       .Select(x => x).FirstOrDefault();
            if (storeContext == null)
            {
                StoreInventory newItem = new StoreInventory();
                newItem.StoreID = stockRequestToUpdate.StoreID;
                newItem.ProductID = stockRequestToUpdate.ProductID;
                newItem.StockLevel = stockRequestToUpdate.Quantity;
                _context.StoreInventory.Add(newItem);
            }
            else
            {
                storeContext.StockLevel += stockRequestToUpdate.Quantity;

            }
            ownerContext.StockLevel -= stockRequestToUpdate.Quantity;
                
            // Delete stock request 
            _context.Remove(stockRequestToUpdate);

            // Submit the changes to the database.
            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Provide for exceptions.
            }

            return RedirectToAction(nameof(DisplayRequests));

        }




    }
}