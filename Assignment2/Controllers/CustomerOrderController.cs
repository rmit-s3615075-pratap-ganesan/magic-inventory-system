using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using Assignment2.Models;
using Assignment2.Models.OrderHistoryModels;

namespace Assignment2.Controllers
{
    public class CustomerOrderController : Controller
    {
        // GET: /<controller>/
        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new HistoryGroupData();
            using (var client = new HttpClient())
            {
                var result = await client.GetStringAsync("http://localhost:5000/api/orders");
                viewModel.customerOrders = JsonConvert.DeserializeObject<List<CustomerOrder>>(result);

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

