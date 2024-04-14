using Microsoft.Extensions.Logging;
using PayToPhone.Driver.App.Contracts.Integrator.Events;
using PayToPhone.Driver.App.Contracts.Integrator.Repository;
using PayToPhone.Driver.App.Contracts.Listener;

namespace PayToPhone.Driver.App.AppServices.Integrator {
    internal class OrderStatusHandler : IOrderStatusHandler {
        private readonly IPayToPhoneRepository _payToPhoneRepository;
        private readonly ILogger _logger;
        private static readonly SemaphoreSlim _semaphoreSlim = new(1);

        public OrderStatusHandler(
            IPayToPhoneRepository payToPhoneRepository,
            ILogger<OrderStatusHandler> logger
            ) {
            _payToPhoneRepository = payToPhoneRepository;
            _logger = logger;
        }

        public async Task OrderStatusChanged(IWebSocketMessege webSocketMessege, CancellationToken cancellationToken) {
            if (webSocketMessege.MessageType == nameof(OrderStatusChanged)) {
                _logger.LogInformation($"orderStatusChanged: {webSocketMessege}");
                var paymentOrderStatusChanged = webSocketMessege.MessageBody.ToObject<OrderStatusChanged>();

                try {
                    await _semaphoreSlim.WaitAsync(cancellationToken);
                    
                    await _payToPhoneRepository.UpdateOrderStatus(
                        paymentOrderStatusChanged.OrderId,
                        paymentOrderStatusChanged.OrderStatus,
                        paymentOrderStatusChanged.Description,
                        cancellationToken);

                } finally {
                    _semaphoreSlim.Release();
                }

            } else {
                _logger.LogError($"Bed event: {webSocketMessege}");
            }
        }
    }
}
