using System;
using System.Collections.Generic;
namespace Assignment2.Models
{
    public class Store
    {
        public int StoreID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<StoreInventory> StoreInventory { get; } = new List<StoreInventory>();
    }
}
