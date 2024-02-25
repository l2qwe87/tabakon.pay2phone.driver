using System;

namespace PayToPhone.Driver.App.Contracts.Integrator
{
    public class PayToPhoneIntegratorResponse
    {
        public Guid PaymentOrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Message { get; set; }
    }
}
