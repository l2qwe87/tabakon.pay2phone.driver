using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace PayToPhone.Driver.App.Contracts.Listener {
    public interface IWebSocketMessege {

        string MessageType { get; }

        JObject MessageBody { get; }
    }
}
