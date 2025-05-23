using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class KitchenActivity
    {
        [FunctionName("PreparePizza")]
        public static async Task<string> PreparePizza(
            [ActivityTrigger] string orderId,
            ILogger log)
        {
            log.LogInformation($"Preparing pizza for order {orderId}.");

            // Simulate pizza preparation
            await Task.Delay(5000); // Simulate a delay

            return $"Pizza for order {orderId} is ready!";
        }
    }
} 