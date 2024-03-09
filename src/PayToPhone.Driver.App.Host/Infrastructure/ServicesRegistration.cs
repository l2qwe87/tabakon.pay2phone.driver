using PayToPhone.Driver.App.AppServices.Infrastructure;

namespace PayToPhone.Driver.App.Host.Infrastructure {
    public static class ServicesRegistration {
        public static IServiceCollection AddAppServices(this IServiceCollection services) {
            services.AddPayToPhone();
            services.AddPayToPhoneIntegratorAsMock();

            return services;
        }
    }
}
