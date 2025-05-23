using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AzureDurableFunctions.PizzaOrderingSystem.Services;

[assembly: FunctionsStartup(typeof(AzureDurableFunctions.PizzaOrderingSystem.Startup))]

namespace AzureDurableFunctions.PizzaOrderingSystem
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IOrderService, OrderService>();
            builder.Services.AddLogging();
        }
    }
}
