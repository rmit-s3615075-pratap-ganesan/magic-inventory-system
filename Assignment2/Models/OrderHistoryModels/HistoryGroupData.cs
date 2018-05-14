using System;
using Assignment2.Models;
using System.Collections.Generic;
namespace Assignment2.Models.OrderHistoryModels
{
    public class HistoryGroupData
    {
        public IEnumerable<CustomerOrder> customerOrders { get; set; }
        public IEnumerable<OrderHistory> orderHistories { get; set; }

    }
}
