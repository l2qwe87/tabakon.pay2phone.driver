using Microsoft.AspNetCore.Mvc;
using PayToPhone.Driver.App.Contracts.Integrator;

namespace PayToPhone.Driver.App.Host.Controllers {

    [ApiController]
    [Route("PayToPhoneIntegratorMock")]
    public class PayToPhoneIntegratorMockController : ControllerBase {

        private readonly IPayToPhoneIntegrator _payToPhoneIntegrator;
        private readonly ILogger<PayToPhoneListenerController> _logger;

        public PayToPhoneIntegratorMockController(
                ILogger<PayToPhoneListenerController> logger,
                IPayToPhoneIntegrator payToPhoneIntegrator
            ) {
            _logger = logger;
            _payToPhoneIntegrator = payToPhoneIntegrator;
        }

        //[HttpPost("PayToPhone")]
        //public Task<PayToPhoneIntegratorResponse> PayToPhone(PayToPhoneIntegratorRequest request, CancellationToken cancellationToken) {
        //    return _payToPhoneIntegrator.PayToPhone(request, cancellationToken);
        //}
    }
}
