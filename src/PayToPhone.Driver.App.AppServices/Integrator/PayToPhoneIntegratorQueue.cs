using Microsoft.Extensions.Logging;
using PayToPhone.Driver.App.Contracts.Integrator;
using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using PayToPhone.Driver.App.Contracts.Integrator.Repository;
using PayToPhone.Driver.App.Contracts.Integrator.Requests;
using PayToPhone.Driver.App.Contracts.Integrator.Responses;

namespace PayToPhone.Driver.App.AppServices.Integrator {
    internal class PayToPhoneIntegratorQueue : IPayToPhoneIntegrator {

        private readonly ILogger _logger;
        private readonly IPayToPhoneRepository _payToPhoneRepository;
        private IMessageQueue _messageQueue;

        private static RefundCommand refundClearCommand = new RefundCommand { Amount = 111, PaymentMethod = Contracts.PaymentMethod.NFC, OrderId = "1111111" };
        private static CreatePaymentOrderCommand createPaymentOrderCleanCommand = new CreatePaymentOrderCommand { Amount = 111, PaymentMethod = Contracts.PaymentMethod.NFC, OrderId = "1111111" };

        public PayToPhoneIntegratorQueue(
            ILogger<PayToPhoneIntegratorWebSocketProxy> logger,
            IPayToPhoneRepository payToPhoneRepository,
            IMessageQueue messageQueue) {
            _logger = logger;
            _payToPhoneRepository = payToPhoneRepository;
            _messageQueue = messageQueue;
        }

        public async Task CreatePaymentOrder(CreatePaymentOrderCommand command, CancellationToken cancellationToken) {
            _messageQueue.Enqueue(refundClearCommand);
            _messageQueue.Enqueue(command);
            _logger.LogInformation($"{typeof(CreatePaymentOrderCommand).Name}: {command}");
            await _payToPhoneRepository.CreatePaymentOrder(command, cancellationToken);
        }

        public Task<GetOrderStatusResponse> GetOrderStatus(GetOrderStatusRequest request, CancellationToken cancellationToken) {
            return _payToPhoneRepository.GetOrderStatus(request, cancellationToken);
        }

        public async Task Refund(RefundCommand command, CancellationToken cancellationToken) {
            _messageQueue.Enqueue(createPaymentOrderCleanCommand);
            _messageQueue.Enqueue(command);
            _logger.LogInformation($"{typeof(RefundCommand).Name}: {command}");
            await _payToPhoneRepository.CreateRefundOrder(command, cancellationToken);
        }
    }
}
