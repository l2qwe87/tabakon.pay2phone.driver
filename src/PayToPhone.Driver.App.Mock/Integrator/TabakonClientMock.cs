using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PayToPhone.Driver.App.AppServices.Listener;
using PayToPhone.Driver.App.Contracts;
using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using PayToPhone.Driver.App.Contracts.Integrator.Events;
using PayToPhone.Driver.App.Contracts.Listener;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Terminal.Gui;
using static PayToPhone.Driver.App.Mock.Integrator.TabakonClientMock;

namespace PayToPhone.Driver.App.Mock.Integrator {
    internal class TabakonClientMock : Window {
        private readonly TextView txtLogs;
        private readonly TextField hostText;
        private readonly ClientWebSocket _clientWebSocket = new ClientWebSocket();
        private Task _receiveLoop;
        private PaymentOrder _lastOrder;
        public TabakonClientMock() {

            var hostLabel = new Label() {
                Text = "host:"
            };

            hostText = new TextField("192.168.88.60:5511") {
                // Position text field adjacent to the label
                X = Pos.Right(hostLabel) + 1,

                // Fill remaining horizontal space
                Width = Dim.Fill(),
            };

            var btnConnect = new Button() {
                Text = "Подключиться",
                Y = Pos.Bottom(hostLabel) + 1,
                ColorScheme = new ColorScheme {
                    Normal = new Terminal.Gui.Attribute(Color.White, Color.Green)
                },
            };

            var btnDisconnect = new Button() {
                Text = "Отключиться",
                Y = Pos.Bottom(hostLabel) + 1,
                X = Pos.Right(btnConnect) + 1,
                ColorScheme = new ColorScheme { 
                    Normal = new Terminal.Gui.Attribute(Color.White , Color.Red)
                },
            };

            txtLogs = new TextView() {
                Y = Pos.Bottom(btnConnect) + 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true,
            };

            btnConnect.Clicked += () => {
                hostText.FocusFirst();
                Log("btnConnect");
                _receiveLoop = Start();
            };

            btnDisconnect.Clicked += () => {
                hostText.FocusFirst();
                Log("btnDisconnect");
            };

            Add(hostLabel, hostText, btnConnect, btnDisconnect, txtLogs);

        }

        public async Task Start(CancellationToken cancellationToken = default) {
            if (_clientWebSocket.State != WebSocketState.Open) {
                var serverUri = new Uri($"ws://{hostText.Text}/");
                await _clientWebSocket.ConnectAsync(serverUri, cancellationToken);

                _receiveLoop = Task.Run(async () => {
                    while (_clientWebSocket.State == WebSocketState.Open) {
                        var buffer = new byte[1024 * 4];
                        var resultRaw = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        var message = Encoding.UTF8.GetString(buffer, 0, resultRaw.Count);

                        var webSocketMessege = JsonConvert.DeserializeObject<WebSocketMessege>(message);
                        Log($"Received :\n {message}");




                        if (webSocketMessege.MessageType == nameof(CreatePaymentOrderCommand)) {
                            var createPaymentOrderCommand = webSocketMessege.MessageBody.ToObject<CreatePaymentOrderCommand>();

                            await SendOrderStatusChanged(createPaymentOrderCommand.OrderId, OrderStatus.Created);

                            if (_lastOrder != null) {
                                Remove(_lastOrder);
                            }
                            _lastOrder = new PaymentOrder(createPaymentOrderCommand, (sender, s) => {
                                SendOrderStatusChanged(s.OrderId, s.OrderStatus, s.Description);
                                _lastOrder = null;
                                Remove(sender);
                            }) { 
                                Title = nameof(CreatePaymentOrderCommand),
                                X = 3,
                                Y = 6,
                                Width = this.Width - 3,
                                Height = this.Height - 3
                            };
                            if (_lastOrder != null) {
                                Application.MainLoop.Invoke(() => Add(_lastOrder));
                            }

                            //    await Task.Delay(TimeSpan.FromSeconds(1));
                            //    await SendStatus(createPaymentOrderCommand.OrderId, OrderStatus.Created);

                            //    await Task.Delay(TimeSpan.FromSeconds(5));
                            //    await SendRandomStatus(createPaymentOrderCommand.OrderId, OrderStatus.Successful);
                        } else if(webSocketMessege.MessageType == nameof(RefundCommand)) {
                            var refundCommand = webSocketMessege.MessageBody.ToObject<RefundCommand>();

                            await SendOrderStatusChanged(refundCommand.OrderId, OrderStatus.Created);

                        } else {
                            Log($"Bad MessageType : {webSocketMessege}");
                        }
                    }
                });

                Log("PayToPhoneIntegratorMock is started");
            }
        }

        private async Task SendOrderStatusChanged(string orderId, OrderStatus orderStatus, string description = null) {
            var webSocketMessege = new WebSocketMessege();
            webSocketMessege.MessageType = nameof(OrderStatusChanged);

            webSocketMessege.MessageBody = JObject.FromObject(new OrderStatusChanged {
                OrderId = orderId,
                OrderStatus = orderStatus,
                Description = description,
            });

            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(webSocketMessege));
            await _clientWebSocket.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void Log(string msg) {
            Application.MainLoop.Invoke(() => txtLogs.Text = $"[{DateTime.Now.ToString("mm:ss.fff")}] {msg} \n{txtLogs.Text}");
        }



        internal class PaymentOrder : FrameView { 
            public PaymentOrder(CreatePaymentOrderCommand createPaymentOrderCommand, Action<View, OrderStatusChanged> callback): base() {
                var orderIdLabel = new Label() {
                    Text = "OrderId : "
                };
                
                var paymentMethodLabel = new Label() {
                    Text = "Payment Method : ",
                    Y = Pos.Bottom(orderIdLabel) + 1,
                };

                var amountLabel = new Label() {
                    Text = "Amount : ",
                    Y = Pos.Bottom(paymentMethodLabel) + 1,
                };

                var descriptionLabel = new Label() {
                    Text = "Description : ",
                    Y = Pos.Bottom(amountLabel) + 1,
                };

                var orderIdText = new TextField(createPaymentOrderCommand.OrderId) {
                    X = Pos.Right(orderIdLabel) + 1,
                    Width = Dim.Fill(),
                    ReadOnly = true,
                };               

                var paymentMethodText = new TextField(createPaymentOrderCommand.PaymentMethod.ToString()) {
                    X = Pos.Right(paymentMethodLabel) + 1,
                    Y = Pos.Bottom(orderIdText) + 1,
                    Width = Dim.Fill(),
                    ReadOnly = true,
                };

                var amountText = new TextField(createPaymentOrderCommand.Amount.ToString()) {
                    X = Pos.Right(paymentMethodLabel) + 1,
                    Y = Pos.Bottom(paymentMethodText) + 1,
                    Width = Dim.Fill(),
                    ReadOnly = true,
                };

                

                var descriptionText = new TextField() {
                    X = Pos.Right(paymentMethodLabel) + 1,
                    Y = Pos.Bottom(amountText) + 1,
                    Width = Dim.Fill(),
                };

                var btnOk = new Button() {
                    Text = "OK",
                    Y = Pos.Bottom(descriptionText) + 1,
                    ColorScheme = new ColorScheme {
                        Normal = new Terminal.Gui.Attribute(Color.White, Color.Green)
                    },
                };

                var btnCancel = new Button() {
                    Text = "CANCEL",
                    X = Pos.Right(btnOk) + 1,
                    Y = Pos.Bottom(descriptionText) + 1,
                    ColorScheme = new ColorScheme {
                        Normal = new Terminal.Gui.Attribute(Color.White, Color.Red)
                    },
                };


                btnOk.Clicked += () => {
                    callback.Invoke(this, new() {
                        OrderId = createPaymentOrderCommand.OrderId,
                        OrderStatus = OrderStatus.Successful,
                        Description = descriptionText.Text.ToString(),
                    });
                };

                btnCancel.Clicked += () => {
                    callback.Invoke(this, new() {
                        OrderId = createPaymentOrderCommand.OrderId,
                        OrderStatus = OrderStatus.Fail,
                        Description = descriptionText.Text.ToString(),
                    });
                };

                Add(orderIdLabel, orderIdText, paymentMethodLabel, paymentMethodText, amountLabel, amountText, descriptionLabel, descriptionText, btnOk, btnCancel);
            }
        }
    }
}
