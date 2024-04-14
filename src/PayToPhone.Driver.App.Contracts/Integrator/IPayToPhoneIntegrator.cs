using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using PayToPhone.Driver.App.Contracts.Integrator.Requests;
using PayToPhone.Driver.App.Contracts.Integrator.Responses;
using PayToPhone.Driver.App.Contracts.Listener;
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

    public interface IMessageQueue {
        void Enqueue(IMessage messege);
        IMessage Dequeue();
    }

    public interface IMessage {
        string OrderId { get; set; }
        PaymentMethod PaymentMethod { get; set; }
        decimal Amount { get; set; }
    }
}
