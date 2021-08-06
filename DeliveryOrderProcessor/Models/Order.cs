using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryOrderProcessor.Models
{
    public class Order: BaseEntity
    {
        public string BuyerId { get;  set; }
        public DateTimeOffset OrderDate { get;  set; } = DateTimeOffset.Now;
        public Address ShipToAddress { get;  set; }

        public List<OrderItem> OrderItems = new List<OrderItem>();

        public decimal Total 
        {
            get
            {
                var total = 0m;
                foreach (var item in OrderItems)
                {
                    total += item.UnitPrice * item.Units;
                }
                return total;
            }
        }
    }
}
