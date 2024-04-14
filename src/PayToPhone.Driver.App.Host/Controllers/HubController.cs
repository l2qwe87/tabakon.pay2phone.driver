using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PayToPhone.Driver.App.AppServices.Listener;
using PayToPhone.Driver.App.Contracts.Integrator;
using PayToPhone.Driver.App.Contracts.Integrator.Repository;
using PayToPhone.Driver.App.Contracts.Listener;

namespace PayToPhone.Driver.App.Host.Controllers {

    [ApiController]
    [Route("Hub")]
    public class HubController : ControllerBase {

        private IMessageQueue _messageQueue;
        private IOrderStatusHandler _orderStatusHandler;

        public HubController(
            IMessageQueue messageQueue,
            IOrderStatusHandler orderStatusHandler) {
            _messageQueue = messageQueue;
            _orderStatusHandler = orderStatusHandler;
        }

        [HttpGet("DequeueCommand")]
        public IWebSocketMessege DequeueCommand() {

            var message = _messageQueue.Dequeue();

            if (message == null) { 
                return null;
            }

            var webSocketMessege = new WebSocketMessege {
                MessageBody = JObject.FromObject(message),
                MessageType = message.GetType().Name,
            };
            return webSocketMessege;
        }

        [HttpPost("OrderStatusChange")]
        public Task OrderStatusChange([FromBody] WebSocketMessege messege, CancellationToken cancellationToken) {
            return _orderStatusHandler.OrderStatusChanged(messege, cancellationToken);
        }

    }
}
