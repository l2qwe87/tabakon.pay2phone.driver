using System;

namespace PayToPhone.Driver.App.Contracts.Integrator.Events {
    public class OrderStatusChanged {
        public string OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Description { get; set; }
    }
}
