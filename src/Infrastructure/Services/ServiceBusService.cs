using Microsoft.Azure.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Services
{
    public class ServiceBusService : IServiceBusService
    {
        private string _serviceBusConnectionString;
        private string _queueName;
        private IQueueClient _queueClient;

        public ServiceBusService(string serviceBusConnectionString, string queueName)
        {
            _serviceBusConnectionString = serviceBusConnectionString;
            _queueName = queueName;
        }

        public async Task SendSalesMessageAsync(string messageBody)
        {
            _queueClient = new QueueClient(_serviceBusConnectionString, _queueName);
            // Send messages.
            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                // Write the body of the message to the console.
                Console.WriteLine($"Sending message: {messageBody}");

                // Send the message to the queue.
                await _queueClient.SendAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }

            await _queueClient.CloseAsync();
        }
    }
}
