using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Terminal.Gui;

namespace PayToPhone.Driver.App.Mock {
    internal class Program {
        static void Main(string[] args) {

            var defaultSerializerSettings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = { new StringEnumConverter() }
            };

            JsonConvert.DefaultSettings = () => defaultSerializerSettings;

            Application.Run<PayToPhoneWindow>();

            // Before the application exits, reset Terminal.Gui for clean shutdown
            Application.Shutdown();
        }
    }
}
