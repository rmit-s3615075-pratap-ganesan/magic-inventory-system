using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class StoreInventory
    {
        public int StoreID { get; set; }
        public Store Store { get; set; }

        public int ProductID { get; set; }
        public Product Product { get; set; }
        [Required(ErrorMessage = "Pleasee enter a number")]
        [Range(1, int.MaxValue , ErrorMessage = "Enter a number greater than 0.")]
        public int StockLevel { get; set; }
    }
}
