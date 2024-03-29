﻿using System;

namespace PayToPhone.Driver.App.Contracts.Integrator.Commands {
    public class CreatePaymentOrderCommand {
        public string OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
    }
}
