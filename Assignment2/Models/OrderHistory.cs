using System;
namespace Assignment2.Models
{
    public class OrderHistory
    {

        public int ReceiptID { get; set; }
        public CustomerOrder CustomerOrder { get; set; }
        public string ProductName { get; set; }
        public string StoreName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
