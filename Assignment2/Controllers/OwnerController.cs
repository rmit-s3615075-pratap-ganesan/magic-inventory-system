using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Assignment2.Controllers
{
    public class OwnerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OwnerController(ApplicationDbContext context)
        {
            _context = context;
        }


        // Auto-parsed variables coming in from the request - there is a form on the page to send this data.
        public async Task<IActionResult> Index(
            string sortOrder, string currentFilter,
            string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["QtySortParm"] = sortOrder == "Qty" ? "qty_desc" : "Qty";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;



            // Eager loading the Product table - join between OwnerInventory and the Product table.
            var query = _context.OwnerInventory.Include(x => x.Product).Select(x => x);

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(x => x.Product.Name.Contains(searchString));

            }

            switch (sortOrder)
            {
                case "name_desc":
                    query = query.OrderByDescending(s => s.Product.Name);
                    break;
                case "Qty":
                    query = query.OrderBy(s => s.StockLevel);
                    break;
                case "date_desc":
                    query = query.OrderByDescending(s => s.StockLevel);
                    break;
                default:
                    query = query.OrderBy(s => s.Product.Name);
                    break;
            }

            int pageSize = 3;
            //var viewModel = new OwnerInventoryViewModel
            //{
            //    Inventory = await PaginatedList<OwnerInventory>
            //       .CreateAsync(query.AsNoTracking(), page ?? 1, pageSize)
            //};

            //// Passing a List<OwnerInventory> model object to the View.
            //return View(viewModel);

            return View(await PaginatedList<OwnerInventory>
                        .CreateAsync(query.AsNoTracking(), page ?? 1, pageSize));
        }


        public String Welcome()
        {
            return "this is a owner page";
        }
    }

}
