using PayToPhone.Driver.App.AppServices;
using PayToPhone.Driver.App.AppServices.Integrator;
using PayToPhone.Driver.App.Contracts;
using PayToPhone.Driver.App.Contracts.Integrator;

namespace PayToPhone.Driver.App.Host.Infrastructure {
    public static class ServicesRegistration {
        public static IServiceCollection AddPayToPhone(this IServiceCollection services) {
            services.AddSingleton<IPayToPhoneListener, PayToPhoneListener>();

            return services;
        }


        public static IServiceCollection AddPayToPhoneIntegratorAsMock(this IServiceCollection services) {
            services.AddSingleton<IPayToPhoneIntegrator, PayToPhoneIntegratorMock>();

            return services;
        }
    }
}
