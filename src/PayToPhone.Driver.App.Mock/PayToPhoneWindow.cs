using PayToPhone.Driver.App.Mock.Integrator;
using Terminal.Gui;


namespace PayToPhone.Driver.App.Mock {
    internal class PayToPhoneWindow : Window {
        public PayToPhoneWindow() {
            Title = "Pay To Phone Mock (Ctrl+Q для выхода)";

            var tabakonClientMock = new TabakonClientMock();

            Add(tabakonClientMock);
        }
    }
}
