using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.Text;
using System.Net.Http;

namespace OrderItemsReserver
{
    public static class UploadOrderToBlob
    {
        private static HttpClient _httpClient;
        private static string _url;

        [FunctionName("UploadOrderToBlob")]
        public static async Task RunAsync(
            [ServiceBusTrigger("orders", Connection = "ServiceBusConnection")]string queueOrder, 
            ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {queueOrder}");

            try
            {
                _url = Environment.GetEnvironmentVariable("OrderItemsReserverLogicAppUrl");
                _httpClient = new HttpClient();

                var order = JsonConvert.DeserializeObject<Order>(queueOrder);

                var fileName = $"Order-{order.Id}.json";

                string orderStorageAccountConnectionString = Environment.GetEnvironmentVariable("OrderStorageAccountConnectionString");

                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(orderStorageAccountConnectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("orders");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                blobClient.DefaultRequestOptions =
                    new BlobRequestOptions()
                    {
                        RetryPolicy = new Microsoft.Azure.Storage.RetryPolicies.LinearRetry(TimeSpan.FromSeconds(10), 3)
                    };

                await blockBlob.UploadFromStreamAsync(new MemoryStream(Encoding.UTF8.GetBytes(queueOrder)));
            }
            catch (Exception ex)
            {
                log.LogError($"UploadOrderToBlob function  Exception: {ex.ToString()}");

                await SendErrorToLogicAppAsync(queueOrder);
            }
        }

        private static async Task<bool> SendErrorToLogicAppAsync(string orderJson)
        {
            var content = new StringContent(orderJson, Encoding.UTF8, "application/json");

            using (var message = await _httpClient.PostAsync(_url, content))
            {
                if (!message.IsSuccessStatusCode)
                {
                    return false;
                }
            }
            return true;

        }

        private class Order
        {
            public int Id { get; set; }
        }
    }
}
