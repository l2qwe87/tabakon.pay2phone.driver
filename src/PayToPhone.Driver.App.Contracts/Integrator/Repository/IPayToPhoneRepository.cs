using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using System.Threading.Tasks;
using System.Threading;
using PayToPhone.Driver.App.Contracts.Integrator.Requests;
using PayToPhone.Driver.App.Contracts.Integrator.Responses;

namespace PayToPhone.Driver.App.Contracts.Integrator.Repository
{
    public interface IPayToPhoneRepository
    {
        Task CreatePaymentOrder(CreatePaymentOrderCommand paymentOrder, CancellationToken cancellationToken);

        Task CreateRefundOrder(RefundCommand paymentOrder, CancellationToken cancellationToken);

        Task UpdateOrderStatus(string orderId, OrderStatus orderStatus, string description, CancellationToken cancellationToken);

        Task<GetOrderStatusResponse> GetOrderStatus(GetOrderStatusRequest getOrderStatusRequest, CancellationToken cancellationToken);
    }
}
