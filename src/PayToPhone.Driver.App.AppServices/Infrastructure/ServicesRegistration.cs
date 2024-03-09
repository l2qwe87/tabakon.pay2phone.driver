using Microsoft.Extensions.DependencyInjection;
using PayToPhone.Driver.App.AppServices.Integrator;
using PayToPhone.Driver.App.AppServices.Listener;
using PayToPhone.Driver.App.Contracts.Integrator;
using PayToPhone.Driver.App.Contracts.Integrator.Repository;
using PayToPhone.Driver.App.Contracts.Listener;

namespace PayToPhone.Driver.App.AppServices.Infrastructure {
    public static class AppServicesRegistration {
        public static IServiceCollection AddPayToPhone(this IServiceCollection services) {
            services.AddSingleton<ITabakonWebSocketServer, TabakonWebSocketServer>();
            services.AddSingleton<IPayToPhoneIntegrator, PayToPhoneIntegratorProxy>();
            services.AddSingleton<IPayToPhoneRepository, PayToPhoneRepository>();

            return services;
        }

        public static IServiceCollection AddPayToPhoneIntegratorAsMock(this IServiceCollection services) {
            services.AddSingleton<PayToPhoneIntegratorMock, PayToPhoneIntegratorMock>();

            return services;
        }
    }
}
