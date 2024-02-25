using PayToPhone.Driver.App.Contracts.Integrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace PayToPhone.Driver.App.AppServices.Integrator {
    public class PayToPhoneIntegratorMock : IPayToPhoneIntegrator {

        private ClientWebSocket clientWebSocket = new ClientWebSocket();
        public PayToPhoneIntegratorMock() {

        }

        public async Task<PayToPhoneIntegratorResponse> PayToPhone(PayToPhoneIntegratorRequest request, CancellationToken cancellationToken) {

            if (clientWebSocket.State != WebSocketState.Open) {
                var serverUri = new Uri(IPAddress.Loopback, 5588);
                await clientWebSocket.ConnectAsync(, cancellationToken);
            }

        }
    }
}
