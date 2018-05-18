using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using Assignment2.Models;
using Assignment2.Models.OrderHistoryModels;
using Assignment2.Data;
using Microsoft.AspNetCore.Identity;

namespace Assignment2.Controllers
{
    public class CustomerOrderController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;


        public CustomerOrderController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(int? id)
        {
            var user = await _userManager.GetUserAsync(User);


            var viewModel = new HistoryGroupData();
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync("http://localhost:5000/api/orders");
                viewModel.customerOrders = JsonConvert.DeserializeObject<List<CustomerOrder>>(result);
                viewModel.customerOrders = viewModel.customerOrders.Select(x => x).Where(x => x.UserEmail == user.Email).Reverse();

            }

            if (id != null)
            {
                ViewData["ReceiptID"] = id.Value;
                using (var client = new HttpClient())
                {
                    var historyResult = await client.GetStringAsync("http://localhost:5000/api/orderhistory/" + id);
                    viewModel.orderHistories = JsonConvert.DeserializeObject<List<OrderHistory>>(historyResult);
                }
            }

            return View(viewModel);
        }
    }
}