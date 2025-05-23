using System;
using System.Threading.Tasks;
using AzureDurableFunctions.PizzaOrderingSystem.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions.PizzaOrderingSystem.Functions
{
    public static class KitchenActivity
    {
        [FunctionName("PreparePizza")]
        public static async Task<OrderResult> PreparePizza(
            [ActivityTrigger] string orderId,
            ILogger log
        )
        {
            log.LogInformation("Preparing pizza for order {OrderId}", orderId);

            try
            {
                // Simulate pizza preparation
                await Task.Delay(2000);
                return new PizzaPrepared
                {
                    OrderId = orderId,
                    PreparationDetails = "Pizza prepared with extra cheese and pepperoni",
                };
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error preparing pizza for order {OrderId}", orderId);
                return new PreparationError
                {
                    OrderId = orderId,
                    ErrorMessage = "Pizza preparation failed",
                    FailedStep = "Preparation"
                };
            }
        }
    }
}
