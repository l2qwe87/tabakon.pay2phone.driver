using PayToPhone.Driver.App.AppServices;
using PayToPhone.Driver.App.Contracts;

namespace PayToPhone.Driver.App.Host.Infrastructure {
    public static class ServicesRegistration {
        public static IServiceCollection AddPayToPhone(this IServiceCollection services) {
            services.AddScoped<IPayToPhoneListener, PayToPhoneListener>();

            return services;
        }
    }
}
