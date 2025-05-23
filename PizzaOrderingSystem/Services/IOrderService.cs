using System.Threading.Tasks;
using AzureDurableFunctions.PizzaOrderingSystem.Models;

namespace AzureDurableFunctions.PizzaOrderingSystem.Services
{
    public interface IOrderService
    {
        Task<OrderResult> SubmitOrderAsync(PizzaOrder order);
        Task<OrderResult> ProcessPaymentAsync(string orderId);
        Task<OrderResult> PreparePizzaAsync(string orderId);
        Task<OrderResult> UpdateDeliveryStatusAsync(string orderId);
        Task<OrderResult> CompleteOrderAsync(string orderId);
        Task<PizzaOrder> GetOrderAsync(string orderId);
    }
}
