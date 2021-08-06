using System;
using System.Collections.Generic;
using System.Text;

namespace DeliveryOrderProcessor.Models
{
    public class OrderItem: BaseEntity
    {
        public CatalogItemOrdered ItemOrdered { get;  set; }
        public decimal UnitPrice { get;  set; }
        public int Units { get;  set; }
    }
}
