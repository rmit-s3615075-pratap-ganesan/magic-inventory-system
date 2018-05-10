using System;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment2.Controllers.Retail
{
    [Authorize(Roles = "Retail")]
    public class RetailController:Controller
    {
        private readonly ApplicationDbContext _context;

        public RetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Auto-parsed variables coming in from the request - there is a form on the page to send this data.
        public async Task<IActionResult> Retail(string productName)
        {
            // Eager loading the Product table - join between OwnerInventory and the Product table.
            var query = _context.StoreInventory.Include(x => x.Product).Select(x => x).Where(x => x.Store.StoreID.Equals(1));

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

        public IActionResult Dashboard()
        {
            return View();
        }


    }
}
