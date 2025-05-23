using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureDurableFunctions
{
    public static class BasicOrchestration
    {
        [FunctionName("BasicOrchestration")]
        public static async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context
        )
        {
            var outputs = new List<string>();
            outputs.Add(
                await context.CallActivityAsync<string>("BasicOrchestration_Hello", "Tokyo")
            );
            outputs.Add(
                await context.CallActivityAsync<string>("BasicOrchestration_Hello", "Seattle")
            );
            outputs.Add(
                await context.CallActivityAsync<string>("BasicOrchestration_Hello", "London")
            );
            return outputs;
        }

        [FunctionName("BasicOrchestration_Hello")]
        public static string SayHello([ActivityTrigger] string name, ILogger log)
        {
            log.LogInformation($"Saying hello to {name}.");
            return $"Hello, {name}!";
        }

        [FunctionName("BasicOrchestration_HttpStart")]
        public static async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log
        )
        {
            string instanceId = await starter.StartNewAsync("BasicOrchestration", null);
            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
