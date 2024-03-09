using Newtonsoft.Json.Linq;
using PayToPhone.Driver.App.Contracts.Listener;

namespace PayToPhone.Driver.App.AppServices.Listener {
    internal class WebSocketMessege : IWebSocketMessege {

        public string MessageType { get; set; }

        public JObject MessageBody { get; set; }
    }
}
