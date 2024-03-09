using Microsoft.AspNetCore.Mvc;
using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using PayToPhone.Driver.App.Contracts.Integrator.Requests;
using PayToPhone.Driver.App.Contracts.Integrator;
using PayToPhone.Driver.App.Contracts.Integrator.Responses;

namespace PayToPhone.Driver.App.Host.Controllers {
    public class PayToPhoneController : ControllerBase {
        private readonly IPayToPhoneIntegrator _payToPhoneIntegrator;

        public PayToPhoneController(
            IPayToPhoneIntegrator payToPhoneIntegrator
            ) {
            _payToPhoneIntegrator = payToPhoneIntegrator;
        }

        [HttpPost("CreatePaymentOrder")]
        public Task CreatePaymentOrder([FromBody] CreatePaymentOrderCommand createPaymentOrderCommand, CancellationToken cancellationToken) {
            return _payToPhoneIntegrator.CreatePaymentOrder(createPaymentOrderCommand, cancellationToken);
        }

        [HttpPost("CreateRefundOrder")]
        public Task CreateRefundOrder([FromBody] RefundCommand refundCommand, CancellationToken cancellationToken) {
            return _payToPhoneIntegrator.Refund(refundCommand, cancellationToken);
        }

        [HttpPost("GetOrderStatus")]
        public Task<GetOrderStatusResponse> GetOrderStatus([FromBody] GetOrderStatusRequest getOrderStatusRequest, CancellationToken cancellationToken) {
            return _payToPhoneIntegrator.GetOrderStatus(getOrderStatusRequest, cancellationToken);
        }
    }
}
