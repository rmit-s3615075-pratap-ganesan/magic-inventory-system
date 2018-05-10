using System;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Data;
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
        public IActionResult StockRequest(string productName)
        {
            // Eager loading the Product table - join between OwnerInventory and the Product table.
            var query = _context.StockRequest.Include(x => x.Product).Include(x => x.Store).Select(x => x);
          
            return View(query.ToList());
        }

    }
}
