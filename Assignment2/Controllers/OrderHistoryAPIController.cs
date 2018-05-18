﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Assignment2.Models;
using Assignment2.Models.CartViewModels;
using Assignment2.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Assignment2.Controllers
{   [Route("api/orderhistory")]
    public class OrderHistoryAPIController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderHistoryAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<OrderHistory> Get()
        {
            return _context.OrderHistory.ToList<OrderHistory>();
        }


        [HttpGet("{id}")]
        public IEnumerable<OrderHistory> Get(int id){

            return _context.OrderHistory.Where(x => x.ReceiptID == id).ToList<OrderHistory>();
        }
    }
}
