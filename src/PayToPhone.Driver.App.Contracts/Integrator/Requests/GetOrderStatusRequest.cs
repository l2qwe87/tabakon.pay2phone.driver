using System;
using System.Collections.Generic;
using System.Text;

namespace PayToPhone.Driver.App.Contracts.Integrator.Requests {
    public class GetOrderStatusRequest {
        public string OrderId { get; set; }
    }
}
