using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IServiceBusService
    {
        Task SendSalesMessageAsync(string messageBody);
    }
}
