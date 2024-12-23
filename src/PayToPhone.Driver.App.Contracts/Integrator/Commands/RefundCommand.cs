namespace PayToPhone.Driver.App.Contracts.Integrator.Commands {
    public class RefundCommand : IMessage {
        public string OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public long TransactionId { get; set; }
        public long Mid { get; set; }
        public decimal Amount { get; set; }
    }
}
