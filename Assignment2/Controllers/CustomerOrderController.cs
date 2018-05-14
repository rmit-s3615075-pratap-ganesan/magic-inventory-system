using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using Assignment2.Models;

namespace Assignment2.Controllers
{
    public class CustomerOrderController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            using(var client = new HttpClient())
            {
                var result = await client.GetStringAsync("http://localhost:5000/api/orders");
                var customerOrders = JsonConvert.DeserializeObject<List<CustomerOrder>>(result);
                return View(customerOrders);
            }
           
        }
    }
}

