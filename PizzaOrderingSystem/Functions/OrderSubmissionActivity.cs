using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class OrderSubmissionActivity
    {
        [FunctionName("SubmitOrder")]
        public static async Task<string> SubmitOrder(
            [ActivityTrigger] string orderDetails,
            ILogger log)
        {
            log.LogInformation($"Submitting order: {orderDetails}.");

            // Simulate order submission
            await Task.Delay(1000); // Simulate a delay

            // Generate a unique order ID (for demo purposes)
            string orderId = Guid.NewGuid().ToString();

            return orderId;
        }
    }
} 