using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Assignment2.Models;

namespace Assignment2.Models
{
    public class CustomerOrder
    {
        [Key]
        public int ReceiptID { get; set; }
        public string UserEmail { get; set; }
        public string UserEmail { get; set;}
        public DateTime TransactionDate { get; set; }
        public ICollection<OrderHistory> OrderHistory { get; } = new List<OrderHistory>();

    }
}

