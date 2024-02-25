using Microsoft.AspNetCore.Mvc;
using PayToPhone.Driver.App.Contracts;

namespace PayToPhone.Driver.App.Host.Controllers;

[ApiController]
[Route("PayToPhoneListener")]
public class PayToPhoneListenerController : ControllerBase {
    private readonly IPayToPhoneListener _payToPhoneListener;
    private readonly ILogger<PayToPhoneListenerController> _logger;

    public PayToPhoneListenerController(
        IPayToPhoneListener payToPhoneListener,
        ILogger<PayToPhoneListenerController> logger
        ) {
        _payToPhoneListener = payToPhoneListener;
        _logger = logger;
    }

    [HttpGet("GetlistenerStatus")]
    public Task<bool> GetlistenerStatus() {
        return _payToPhoneListener.GetlistenerStatus();
    }

    [HttpGet("Startlistener")]
    public Task Startlistener() {
        return _payToPhoneListener.Startlistener();
    }

    [HttpGet("Stoplistener")]
    public Task Stoplistener() {
        return _payToPhoneListener.Stoplistener();
    }

    
}
