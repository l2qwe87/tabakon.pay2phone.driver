using PayToPhone.Driver.App.Contracts;
using PayToPhone.Driver.App.Contracts.Integrator.Commands;
using PayToPhone.Driver.App.Contracts.Integrator.Repository;
using PayToPhone.Driver.App.Contracts.Integrator.Requests;
using PayToPhone.Driver.App.Contracts.Integrator.Responses;

namespace PayToPhone.Driver.App.AppServices.Integrator {
    internal class PayToPhoneRepository : IPayToPhoneRepository {

        private InMemoryRepository<string, OrderEntity> orders = new();

        public Task CreatePaymentOrder(CreatePaymentOrderCommand command, CancellationToken cancellationToken) {
            var entity = new OrderEntity {
                OrderId = command.OrderId,
                PaymentMethod = command.PaymentMethod,
                Amount = command.Amount,
                OrderStatus = OrderStatus.New
            };

            orders.AddOrUpdate(command.OrderId, entity);

            return Task.CompletedTask;
        }

        public Task CreateRefundOrder(RefundCommand command, CancellationToken cancellationToken) {
            var entity = new OrderEntity {
                OrderId = command.OrderId,
                PaymentMethod = command.PaymentMethod,
                Amount = command.Amount,
                OrderStatus = OrderStatus.New
            };

            orders.AddOrUpdate(command.OrderId, entity);

            return Task.CompletedTask;
        }

        public Task<GetOrderStatusResponse> GetOrderStatus(GetOrderStatusRequest request, CancellationToken cancellationToken) {
            GetOrderStatusResponse result = null;

            var entity = orders.Get(request.OrderId);

            if (entity != null) {
                result = new() {
                    OrderStatus = entity.OrderStatus,
                    Description = entity.Description
                };
            } else {
                result = new() {
                    OrderStatus = OrderStatus.Fail,
                    Description = $"Order with id [{request.OrderId}] not found"
                };
            }

            return Task.FromResult(result);
        }

        public Task UpdateOrderStatus(string orderId, OrderStatus orderStatus, string description, CancellationToken cancellationToken) {

            var entity = orders.Get(orderId);

            if(entity != null) { 
                entity.OrderStatus = orderStatus;
                entity.Description = description;
            }

            return Task.CompletedTask;
        }

        private class OrderEntity { 
            public string OrderId { get; set; }

            public PaymentMethod PaymentMethod { get; set; }

            public decimal Amount { get; set; }

            public OrderStatus OrderStatus { get; set; }

            public string Description { get; set; }
        }
    }
}
