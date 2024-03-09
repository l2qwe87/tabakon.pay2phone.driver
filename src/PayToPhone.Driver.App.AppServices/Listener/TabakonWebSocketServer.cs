using Newtonsoft.Json;
using PayToPhone.Driver.App.Contracts.Listener;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace PayToPhone.Driver.App.AppServices.Listener {
    internal class TabakonWebSocketServer : ITabakonWebSocketServer {
        
        private readonly HttpListener _listener = new();
        private Thread _threadIncomingClients;
        private bool _status = false;

        private readonly Dictionary<Guid, (WebSocket ws, Task task)> _clientTasks = new Dictionary<Guid, (WebSocket, Task)>();


        public event EventHandler<IWebSocketMessege> OnMessegeReceived;

        private readonly ILogger _logger;

        public TabakonWebSocketServer(
            ILogger<TabakonWebSocketServer> logger
            ) {
            _logger = logger;

            InitThreadIncomingClients();
        }

        public void Startlistener(string uriPrefix) {
            var status = GetlistenerStatus();
            var isStarted = status;
            if (isStarted) {
                throw new Exception();
            }

            _listener.Prefixes.Add(uriPrefix);

            _listener.Start();

            _logger.LogInformation($"TabakonWebSocketServer is started on {uriPrefix}");

            _status = true;
        }

        public void Stoplistener() {
            var status = GetlistenerStatus();
            var isStoped = !status;
            if (isStoped) {
                throw new Exception();
            }

            _listener.Stop();

            _logger.LogInformation("TabakonWebSocketServer is stoped");

            _status = false;
        }

        public bool GetlistenerStatus() {
            return _status;
        }

        public async Task SendMessege(IWebSocketMessege message, CancellationToken cancellationToken) {
            //var clientId = message.ClientId;
            //single client mode
            var clientId = _clientTasks.Last().Key;

            if (_clientTasks.TryGetValue(clientId, out var client)) {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                await client.ws.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, cancellationToken);
            } else { 
                throw new Exception($"ClientId={clientId} not found");
            }
        }


        private void InitThreadIncomingClients() {
            if (_threadIncomingClients == null) {
                lock (this) {
                    if (_threadIncomingClients == null) {
                        var threadRequest = new Thread(async () => {
                            while (true) {
                                if (_status) {
                                    _logger.LogInformation($"WebSocket ready for new conection.");
                                    var context = await _listener.GetContextAsync();
                                    RegistrateClient(context);

                                } else {
                                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                                }
                            }
                        }) { IsBackground = true };
                        threadRequest.Start();
                        _threadIncomingClients = threadRequest;
                    }
                }
            }
        }

        private void RegistrateClient(HttpListenerContext context) {
            if (context.Request.IsWebSocketRequest) {
                lock (_clientTasks) {
                    var clientId = Guid.NewGuid();
                    
                    var task = new Task(async () => {
                        var ws = await context.AcceptWebSocketAsync(subProtocol: null);
                        _clientTasks[clientId] = (ws.WebSocket, _clientTasks[clientId].task);
                        _logger.LogInformation($"WebSocket connected, clientId={clientId}");                           
                        await ReceiverLoop(clientId, ws.WebSocket);
                        _clientTasks.Remove(clientId);
                    });
                    _clientTasks.Add(clientId, (null, task));
                    task.Start();
                }
            } else {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }

        private async Task ReceiverLoop(Guid clientId, WebSocket ws) {
            var buffer = new byte[1024 * 4];
            while (true) {
                try {
                    var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Text) {
                        var messageRaw = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        _logger.LogInformation($"ClientId:{clientId}, Received: " + messageRaw);
                        var message = JsonConvert.DeserializeObject<WebSocketMessege>(messageRaw);
                        OnMessegeReceived?.Invoke(this, message);
                    } else if (result.MessageType == WebSocketMessageType.Close) {
                        _logger.LogInformation($"ClientId:{clientId}, WebSocket closed");
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                        break;
                    }
                } catch (WebSocketException e) {
                    _logger.LogInformation($"ClientId:{clientId}, WebSocketException: {e.Message}");
                    return;
                }
            }
        }
    }
}
