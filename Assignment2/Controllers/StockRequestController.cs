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

        // Auto-parsed variables coming in from the request - there is a form on the page to send this data.
        public IActionResult Index(string productName)
        {
            // Eager loading the Product table - join between OwnerInventory and the Product table.
            var query = _context.StockRequest.Include(x => x.Product).Include(x => x.Store).Select(x => x).ToList();

            return View(query);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var query = _context.StockRequest
                                .Include(x => x.Product)
                                .Include(x => x.Store)
                                 .Where(x => x.StockRequestID == id)
                               .First();

             ViewData["stockAvailability"] = _context.OwnerInventory
                                                    .Where(x => x.ProductID == query.ProductID)
                                                     .Select(x => x.StockLevel).First();

            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }


        public async Task<IActionResult> Process()
        {
            var stockRequestID = Convert.ToInt32("" + Request.Form["StockrequestID"]);
            if (stockRequestID == 0)
            {
                return NotFound();
            }

            var stockRequestToUpdate = _context.StockRequest.Where(x => x.StockRequestID == stockRequestID).First();
            var ownerInventoryToUpdate = _context.OwnerInventory.Select(x=>x).First();
           
             ownerInventoryToUpdate.StockLevel -= stockRequestToUpdate.Quantity;
           
            
            if (await TryUpdateModelAsync<OwnerInventory>(
                ownerInventoryToUpdate,
                "",
                s => s.StockLevel))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }

            return RedirectToAction(nameof(Index));

        }



    }
}
