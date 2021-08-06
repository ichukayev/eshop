﻿using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IDeliveryOrderProcessor
    {
        Task<bool> UploadOrderToOrderProcessorAsync(Order order);
    }
}
