using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class OrderSubmissionActivity
    {
        [FunctionName("SubmitOrder")]
        public static async Task<string> SubmitOrder(
            [ActivityTrigger] JObject orderData,
            ILogger log)
        {
            // Log the detailed order information
            if (orderData != null)
            {
                log.LogInformation($"Submitting order with details:");
                log.LogInformation($"Size: {orderData["size"]}");
                log.LogInformation($"Toppings: {orderData["toppings"]}");
                log.LogInformation($"Address: {orderData["address"]}");
                log.LogInformation($"Phone: {orderData["phone"]}");
            }
            else
            {
                log.LogWarning("Order data is null");
            }

            // Simulate order submission
            await Task.Delay(1000); // Simulate a delay

            // Generate a unique order ID (for demo purposes)
            string orderId = Guid.NewGuid().ToString();

            log.LogInformation($"Order submitted successfully with ID: {orderId}");

            return orderId;
        }
    }
} 