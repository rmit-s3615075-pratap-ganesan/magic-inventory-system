using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Data;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class StockRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public async Task<IActionResult> ViewData(string productName)
        {

            // Eager loading the Product table - join between OwnerInventory and the Product table.
            var query = _context.StockRequest.Include(x => x.Product).Include(x => x.Store).Select(x => x);
            

            if (!string.IsNullOrWhiteSpace(productName))
            {
                // Adding a where to the query to filter the data.
                // Note for the first request productName is null thus the where is not always added.
                query = query.Where(x => x.Product.Name.Contains(productName));

                // Storing the search into ViewBag to populate the textbox with the same value for convenience.
                ViewBag.ProductName = productName;
            }

            // Adding an order by to the query for the Product name.
            //// query = query.OrderBy(x => x.ProductID);

            // Passing a List<StockInventory> model object to the View.
            return View(await query.ToListAsync());
        }


        public String Welcome()
        {
            return "this is a owner page";
        }
    }


}
