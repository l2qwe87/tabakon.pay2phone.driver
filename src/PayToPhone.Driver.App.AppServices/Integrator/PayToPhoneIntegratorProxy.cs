﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayToPhone.Driver.App.AppServices.Listener;
using PayToPhone.Driver.App.Contracts.Integrator;
using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using PayToPhone.Driver.App.Contracts.Integrator.Events;
using PayToPhone.Driver.App.Contracts.Integrator.Repository;
using PayToPhone.Driver.App.Contracts.Integrator.Requests;
using PayToPhone.Driver.App.Contracts.Integrator.Responses;
using PayToPhone.Driver.App.Contracts.Listener;
using System;

namespace PayToPhone.Driver.App.AppServices.Integrator
{
    internal class PayToPhoneIntegratorProxy : IPayToPhoneIntegrator {

        private readonly IPayToPhoneRepository _payToPhoneRepository;
        private readonly ITabakonWebSocketServer _tabakonWebSocketServer;
        private readonly ILogger _logger;

        private string _latestOrderId = null;

        public PayToPhoneIntegratorProxy(
            IPayToPhoneRepository payToPhoneRepository,
            ITabakonWebSocketServer tabakonWebSocketServer,
            ILogger<PayToPhoneIntegratorProxy> logger
            ) {
            _payToPhoneRepository = payToPhoneRepository;
            _tabakonWebSocketServer = tabakonWebSocketServer;
            _logger = logger;

            _tabakonWebSocketServer.OnMessegeReceived += orderStatusChanged;
        }

        public async Task CreatePaymentOrder(CreatePaymentOrderCommand command, CancellationToken cancellationToken) {
            _latestOrderId = command.OrderId;

            await SendCleaner<CreatePaymentOrderCommand>(cancellationToken);
            await SendMessage(command, cancellationToken);
            await _payToPhoneRepository.CreatePaymentOrder(command, cancellationToken);
        }

        public async Task Refund(RefundCommand command, CancellationToken cancellationToken) {
            _latestOrderId = command.OrderId;

            await SendCleaner<RefundCommand>(cancellationToken);
            await SendMessage(command, cancellationToken);
            await _payToPhoneRepository.CreateRefundOrder(command, cancellationToken);
        }

        public Task<GetOrderStatusResponse> GetOrderStatus(GetOrderStatusRequest getOrderStatusRequest, CancellationToken cancellationToken){
            return _payToPhoneRepository.GetOrderStatus(getOrderStatusRequest, cancellationToken);
        }

        private async Task SendCleaner<T>(CancellationToken cancellationToken) {
            if (typeof(T) == typeof(CreatePaymentOrderCommand)) {
                await SendMessage(new RefundCommand { Amount = 111, PaymentMethod = Contracts.PaymentMethod.NFC, OrderId = "1111111" }, cancellationToken);
            }
            if (typeof(T) == typeof(RefundCommand)) {
                await SendMessage(new CreatePaymentOrderCommand { Amount = 111, PaymentMethod = Contracts.PaymentMethod.NFC, OrderId = "1111111" }, cancellationToken);
            }
            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        private Task SendMessage<T>(T message, CancellationToken cancellationToken) {
            _logger.LogInformation($"{typeof(T).Name}: {message}");

            var webSocketMessege = new WebSocketMessege {
                MessageBody = JObject.FromObject(message),
                MessageType = typeof(T).Name,
            };

            return _tabakonWebSocketServer.SendMessege(webSocketMessege, cancellationToken);
        }

        private void orderStatusChanged(object sender, IWebSocketMessege webSocketMessege) {
            if(webSocketMessege.MessageType == nameof(OrderStatusChanged)) {
                _logger.LogInformation($"orderStatusChanged: {webSocketMessege}");
                var paymentOrderStatusChanged = webSocketMessege.MessageBody.ToObject<OrderStatusChanged>();

                //OnPaymentOrderStatusChanged?.Invoke(this, paymentOrderStatusChanged);
                lock (_payToPhoneRepository) {
                    _payToPhoneRepository
                        .UpdateOrderStatus(_latestOrderId, paymentOrderStatusChanged.OrderStatus, paymentOrderStatusChanged.Description, CancellationToken.None)
                        .Wait();
                }

            } else {
                _logger.LogError($"Bed event: {webSocketMessege}");
            }
        }
    }
}
