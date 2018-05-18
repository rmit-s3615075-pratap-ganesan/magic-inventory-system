using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class OwnerInventory
    {
        [Key, ForeignKey("Product"), Display(Name = "Product ID")]
        public int ProductID { get; set; }
        public Product Product { get; set; }

        [Required(ErrorMessage = "Pleasee enter a number")]
        [Range(1, int.MaxValue, ErrorMessage = "Enter a number greater than 0.")]
        [Display(Name = "Stock Level")]
        public int StockLevel { get; set; }
    }
}
