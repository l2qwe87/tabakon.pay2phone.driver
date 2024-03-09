using System;
using System.Collections.Generic;
using System.Text;

namespace PayToPhone.Driver.App.Contracts.Integrator.Responses {
    public class GetOrderStatusResponse {
        public OrderStatus OrderStatus { get; set; }
        
        public string Description { get; set; }
    }
}
