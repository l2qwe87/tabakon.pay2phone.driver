using System;
using System.Threading;
using System.Threading.Tasks;

namespace PayToPhone.Driver.App.Contracts.Listener
{
    public interface ITabakonWebSocketServer {

        event EventHandler<IWebSocketMessege> OnMessegeReceived;
        Task SendMessege(IWebSocketMessege message, CancellationToken cancellationToken);
        void Startlistener(string uriPrefix);
        void Stoplistener();
        bool GetlistenerStatus();
    }
}
