using System.Threading.Tasks;

namespace PayToPhone.Driver.App.Contracts.Integrator
{
    public interface IPayToPhoneIntegrator
    {
        Task<PayToPhoneIntegratorResponse> PayToPhone(PayToPhoneIntegratorRequest request);
    }
}
