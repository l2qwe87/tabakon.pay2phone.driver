using Microsoft.Extensions.Logging;
using PayToPhone.Driver.App.Contracts;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace PayToPhone.Driver.App.AppServices {
    public class PayToPhoneListener : IPayToPhoneListener {

        private bool _status = false;
        private TcpListener _tcpListener;
        private Thread ThreadClients;


        private readonly ILogger<PayToPhoneListener> _logger;

        public PayToPhoneListener(
            ILogger<PayToPhoneListener> logger
            ) {
            _logger = logger;
            _tcpListener = new TcpListener(IPAddress.Any, 5511);
            InitThreadClients();
        }
        public Task<bool> GetlistenerStatus() {            
            return Task.FromResult(_status);
        }

        public async Task Startlistener() {
            var status = await GetlistenerStatus();
            var isStarted = status;
            if (isStarted) {
                throw new Exception();
            }

            _tcpListener.Start();

            _status = true;
        }

        public async Task Stoplistener() {
            var status = await GetlistenerStatus();
            var isStoped = !status;
            if (isStoped) {
                throw new Exception();
            }

            _tcpListener.Stop();

            _status = false;
        }

        private void InitThreadClients() {
            if (ThreadClients != null) {
                lock (this) {
                    if (ThreadClients != null) {
                        var threadClients = new Thread(async () => {
                            while (_status) {
                                if (_status) {
                                    var client = await _tcpListener.AcceptTcpClientAsync();
                                    _logger.LogInformation($"Accept Tcp Client {client}");
                                } else {
                                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                                }
                            }
                        });
                        threadClients.Start();
                        ThreadClients = threadClients;
                    }
                }
            }
        }
    }
}
