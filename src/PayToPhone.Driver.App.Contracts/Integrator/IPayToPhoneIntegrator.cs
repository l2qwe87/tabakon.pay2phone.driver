using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using PayToPhone.Driver.App.Contracts.Integrator.Requests;
using PayToPhone.Driver.App.Contracts.Integrator.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace PayToPhone.Driver.App.Contracts.Integrator
{
    public interface IPayToPhoneIntegrator
    {
        Task CreatePaymentOrder(CreatePaymentOrderCommand command, CancellationToken cancellationToken);

        Task Refund(RefundCommand command, CancellationToken cancellationToken);

        Task<GetOrderStatusResponse> GetOrderStatus(GetOrderStatusRequest request, CancellationToken cancellationToken);
    }
}
