﻿using System;
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
        public IActionResult Index()
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

            ViewData["stockAvailability"] = await _context.OwnerInventory
                                                   .Where(x => x.ProductID == query.ProductID)
                                                    .Select(x => x.StockLevel).FirstAsync();

            if (query == null)
            {
                return NotFound();
            }

            return View(query);
        }


        public IActionResult Process()
        {
            var stockRequestID = Convert.ToInt32("" + Request.Form["StockrequestID"]);

            var productID = Convert.ToInt32("" + Request.Form["ProductID"]);

            if (stockRequestID == 0)
            {
                return NotFound();
            }
            var stockRequestToUpdate = _context.StockRequest.Where(x => x.StockRequestID == stockRequestID).First();
            var ownerContext = _context.OwnerInventory.Where(x => x.ProductID == productID).Select(x => x).First();
            var storeContext = _context.StoreInventory.Where(x => x.ProductID == productID).Select(x => x).First();

            ownerContext.StockLevel -= stockRequestToUpdate.Quantity;
            storeContext.StockLevel += stockRequestToUpdate.Quantity;
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

            return RedirectToAction(nameof(Index));

        }




    }
}