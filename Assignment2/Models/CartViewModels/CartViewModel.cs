using System;
using Assignment2.Models;
using System.Collections.Generic;
namespace Assignment2.Models.CartViewModels
{
    public class CartViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int StoreID { get; set; }
        public string StoreName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }



    }
}
