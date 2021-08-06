using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Net;
using DeliveryOrderProcessor.Models;

namespace DeliveryOrderProcessor
{
    public static class DeliveryOrderProcessor
    {
        [FunctionName("UploadOrderToOrderProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "OrderProcessor",
                collectionName: "orders",
                ConnectionStringSetting = "DeliveryOrderProcessorCosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string orderJson = await new StreamReader(req.Body).ReadToEndAsync();
            var order = JsonConvert.DeserializeObject<Order>(orderJson);

            string responseMessage = await CreateUserDocumentIfNotExists(client, "OrderProcessor", "orders", order, order.Id.ToString());

            return new OkObjectResult(responseMessage);
        }

        private static async Task<string> CreateUserDocumentIfNotExists(DocumentClient client, string databaseName, string collectionName, Order order, string orderId)
        {
            try
            {
                await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(databaseName, collectionName, orderId), new RequestOptions { PartitionKey = new PartitionKey(orderId) });
                return $"Order {orderId} already exists in the database";
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), order);
                    return $"Created order {orderId}";
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
