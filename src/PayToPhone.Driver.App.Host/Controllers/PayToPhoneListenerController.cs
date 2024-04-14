using Microsoft.AspNetCore.Mvc;
using PayToPhone.Driver.App.Contracts.Listener;

namespace PayToPhone.Driver.App.Host.Controllers;

[ApiController]
[Route("PayToPhoneListener")]
public class PayToPhoneListenerController : ControllerBase {
    private readonly ITabakonWebSocketServer _tabakonWebSocketServer;

    public PayToPhoneListenerController(
        ITabakonWebSocketServer tabakonWebSocketServer
        ) {
        _tabakonWebSocketServer = tabakonWebSocketServer;
    }

    [HttpGet("GetlistenerStatus")]
    public bool GetlistenerStatus() {
        return _tabakonWebSocketServer.GetlistenerStatus();
    }

    [HttpGet("Startlistener")]
    public void Startlistener([FromQuery] string host="*", int port= 5511) {
        _tabakonWebSocketServer.Startlistener($"http://{host}:{port}/");
    }

    [HttpGet("Stoplistener")]
    public void Stoplistener() {
        _tabakonWebSocketServer.Stoplistener();
    }

    
}
