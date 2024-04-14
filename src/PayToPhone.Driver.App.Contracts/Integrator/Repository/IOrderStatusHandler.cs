using System.Threading.Tasks;
using System.Threading;
using PayToPhone.Driver.App.Contracts.Listener;

namespace PayToPhone.Driver.App.Contracts.Integrator.Repository {
    public interface IOrderStatusHandler {
        Task OrderStatusChanged(IWebSocketMessege webSocketMessege, CancellationToken cancellationToken);
    }
}
