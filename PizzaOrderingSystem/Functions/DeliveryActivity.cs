using System;
using System.Threading.Tasks;
using AzureDurableFunctions.PizzaOrderingSystem.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class DeliveryActivity
    {
        [FunctionName("UpdateDeliveryStatus")]
        public static async Task<OrderResult> UpdateDeliveryStatus(
            [ActivityTrigger] string orderId,
            ILogger log
        )
        {
            log.LogInformation("Updating delivery status for order {OrderId}", orderId);

            try
            {
                // Simulate delivery status update
                await Task.Delay(1000);
                return new DeliveryUpdated { OrderId = orderId, Status = "Out for delivery" };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error updating delivery status for order {OrderId}", orderId);
                return new DeliveryError { OrderId = orderId, ErrorMessage = "Failed to update delivery status", CurrentLocation = "Unknown" };
            }
        }
    }
}
