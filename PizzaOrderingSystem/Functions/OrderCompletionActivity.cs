using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class OrderCompletionActivity
    {
        [FunctionName("CompleteOrder")]
        public static async Task<string> CompleteOrder(
            [ActivityTrigger] string orderId,
            ILogger log)
        {
            log.LogInformation($"Completing order {orderId}.");

            // Simulate order completion
            await Task.Delay(1000); // Simulate a delay

            return $"Order {orderId} has been completed successfully!";
        }
    }
} 