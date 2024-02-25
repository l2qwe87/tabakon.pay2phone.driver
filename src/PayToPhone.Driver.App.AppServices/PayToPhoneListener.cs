using PayToPhone.Driver.App.Contracts;

namespace PayToPhone.Driver.App.AppServices {
    public class PayToPhoneListener : IPayToPhoneListener {

        private bool _status = false;
        public async Task<bool> GetlistenerStatus() {
            await RandomDelay();
            return true;
        }

        public async Task Startlistener() {
            var status = await GetlistenerStatus();
            if (status) {
                throw new Exception();
            }
            _status = true;
        }

        public async Task Stoplistener() {
            var status = await GetlistenerStatus();
            if (!status) {
                throw new Exception();
            }

            _status = false;
        }


        private Task RandomDelay(int stepMs = 3000) {
            var ms = (Random.Shared.Next() % 3000);
            return Task.Delay(ms);
        }
    }
}
