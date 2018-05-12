using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment2.Models
{
    public class CustomerOrder
    {
        [Key]
        public int ReceiptID { get; set; }
        public string UserEmail { get; set;}
        public DateTime TransactionDate { get; set; }

    }
}
