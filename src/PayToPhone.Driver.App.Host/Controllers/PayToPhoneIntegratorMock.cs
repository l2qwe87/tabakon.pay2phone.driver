using Microsoft.AspNetCore.Mvc;
using PayToPhone.Driver.App.Contracts.Integrator;

namespace PayToPhone.Driver.App.Host.Controllers {

    [ApiController]
    [Route("PayToPhoneIntegratorMock")]
    public class PayToPhoneIntegratorMockController : ControllerBase {

        private readonly ILogger<IPayToPhoneIntegrator> payToPhoneIntegrator;
        private readonly ILogger<PayToPhoneListenerController> _logger;

        public PayToPhoneIntegratorMockController(
                ILogger<PayToPhoneListenerController> logger
            ) {
            _logger = logger;
        }
    }
}
