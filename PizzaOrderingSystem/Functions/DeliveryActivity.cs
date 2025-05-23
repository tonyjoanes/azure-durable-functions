using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class DeliveryActivity
    {
        [FunctionName("UpdateDeliveryStatus")]
        public static async Task<string> UpdateDeliveryStatus(
            [ActivityTrigger] string orderId,
            ILogger log)
        {
            log.LogInformation($"Updating delivery status for order {orderId}.");

            // Simulate delivery status update
            await Task.Delay(3000); // Simulate a delay

            return $"Delivery for order {orderId} is in progress!";
        }
    }
} 