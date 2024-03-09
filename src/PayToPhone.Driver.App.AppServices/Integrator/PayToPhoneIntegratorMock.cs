using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayToPhone.Driver.App.AppServices.Listener;
using PayToPhone.Driver.App.Contracts;
using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using PayToPhone.Driver.App.Contracts.Integrator.Events;
using System.Net.WebSockets;
using System.Text;

namespace PayToPhone.Driver.App.AppServices.Integrator {
    public class PayToPhoneIntegratorMock {


        private readonly ClientWebSocket _clientWebSocket = new ClientWebSocket();

        private Task _receiveLoop;

        private readonly ILogger _logger;
        public PayToPhoneIntegratorMock(
            ILogger<PayToPhoneIntegratorMock> logger
            ) {

            _logger = logger;
        }

        public async Task Start(CancellationToken cancellationToken = default) {
            if (_clientWebSocket.State != WebSocketState.Open) {
                var serverUri = new Uri("ws://192.168.88.60:5511/");
                await _clientWebSocket.ConnectAsync(serverUri, cancellationToken);

                _receiveLoop = Task.Run(async () => { 
                    while(_clientWebSocket.State == WebSocketState.Open) {
                        var buffer = new byte[1024 * 4];
                        var resultRaw = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        var message = Encoding.UTF8.GetString(buffer, 0, resultRaw.Count);

                        var webSocketMessege = JsonConvert.DeserializeObject<WebSocketMessege>(message);
                        _logger.LogInformation($"Received : {webSocketMessege}");
                        if (webSocketMessege.MessageType == nameof(CreatePaymentOrderCommand)) {
                            var createPaymentOrderCommand = webSocketMessege.MessageBody.ToObject<CreatePaymentOrderCommand>();
                            
                            await Task.Delay(TimeSpan.FromSeconds(1));
                            await SendStatus(createPaymentOrderCommand.OrderId, OrderStatus.Created);
                           
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            await SendRandomStatus(createPaymentOrderCommand.OrderId, OrderStatus.Successful);
                        } else {
                            _logger.LogInformation($"Bad MessageType : {webSocketMessege}");
                        }
                    }
                });

                _logger.LogInformation("PayToPhoneIntegratorMock is started");
            }
        }

        private Task SendRandomStatus(string orderId, OrderStatus offset) {
            Random rnd = new Random();
            var rndInt = rnd.Next((int)offset, (int)OrderStatus.Fail);
            var newStatus = (OrderStatus)rndInt;
            return SendStatus(orderId, newStatus);
        }

        private async Task SendStatus(string orderId, OrderStatus status) {
            var paymentOrderStatusChanged = new OrderStatusChanged {
                OrderId = orderId,
                OrderStatus = status,
                Description = $"Statuse is {status}",
            };

            var outMess = new WebSocketMessege { MessageType = nameof(OrderStatusChanged), MessageBody = JObject.FromObject(paymentOrderStatusChanged) };
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(outMess));

            await _clientWebSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
