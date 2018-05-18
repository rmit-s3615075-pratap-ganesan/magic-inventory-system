using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Microsoft.AspNetCore.Authorization;

namespace Assignment2.Controllers
{
    [Authorize(Roles = "WholeSale")]
    public class OwnerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OwnerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Owner
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OwnerInventory.Include(o => o.Product);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Owner Stock Req
        public async Task<IActionResult> DisplayRequests()
        {
            var applicationDbContext = _context.StockRequest.Include(x => x.Product).Select(x => x);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Owner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownerInventory = await _context.OwnerInventory
                .Include(o => o.Product)
                .SingleOrDefaultAsync(m => m.ProductID == id);
            if (ownerInventory == null)
            {
                return NotFound();
            }

            return View(ownerInventory);
        }

        // GET: Owner/Create
        public IActionResult Create()
        {
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID");
            return View();
        }

        // POST: Owner/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductID,StockLevel")] OwnerInventory ownerInventory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ownerInventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", ownerInventory.ProductID);
            return View(ownerInventory);
        }

        // GET: Owner/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownerInventory = _context.OwnerInventory.Include(x => x.Product).Where(m => m.ProductID == id).First();
            if (ownerInventory == null)
            {
                return NotFound();
            }
            //ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", ownerInventory.ProductID);
            return View(ownerInventory);
        }

        // POST: Owner/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,StockLevel")] OwnerInventory ownerInventory)
        {
            if (id != ownerInventory.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ownerInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerInventoryExists(ownerInventory.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", ownerInventory.ProductID);
            return View(ownerInventory);
        }

        // GET: Owner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ownerInventory = await _context.OwnerInventory
                .Include(o => o.Product)
                .SingleOrDefaultAsync(m => m.ProductID == id);
            if (ownerInventory == null)
            {
                return NotFound();
            }

            return View(ownerInventory);
        }

        // POST: Owner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ownerInventory = await _context.OwnerInventory.SingleOrDefaultAsync(m => m.ProductID == id);
            _context.OwnerInventory.Remove(ownerInventory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OwnerInventoryExists(int id)
        {
            return _context.OwnerInventory.Any(e => e.ProductID == id);
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
