﻿using System;
namespace Assignment2.Models
{
    public class StoreInventory
    {
        public int StoreID { get; set; }
        public Store Store { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int StockLevel { get; set; }
    }
}