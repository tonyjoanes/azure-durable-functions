using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AzureDurableFunctions.PizzaOrderingSystem.Models
{
    public class PizzaOrder
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Size { get; set; }

        [Required]
        public List<string> Toppings { get; set; } = new List<string>();

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Created;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }

    public enum OrderStatus
    {
        Created,
        Confirmed,
        PaymentProcessed,
        InKitchen,
        OutForDelivery,
        Delivered,
        Cancelled,
        Failed,
    }
}
