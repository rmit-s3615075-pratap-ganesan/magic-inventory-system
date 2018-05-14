using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Models;
using Assignment2.Models.CartViewModels;
using Assignment2.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Assignment2.Controllers
{
    [Route("api/orders")]
    public class CustomerOrderAPIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerOrderAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IEnumerable<CustomerOrder>> Get()
        {
            return  _context.CustomerOrder.ToList<CustomerOrder>();
        }



    }
}
