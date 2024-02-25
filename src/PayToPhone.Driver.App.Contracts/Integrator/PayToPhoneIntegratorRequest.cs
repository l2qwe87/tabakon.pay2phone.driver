using System;
using System.Collections.Generic;
using System.Text;

namespace PayToPhone.Driver.App.Contracts.Integrator {
    public class PayToPhoneIntegratorRequest {
        public Guid PaymentOrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
