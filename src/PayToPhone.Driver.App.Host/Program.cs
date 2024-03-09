using PayToPhone.Driver.App.AppServices.Integrator;
using PayToPhone.Driver.App.Contracts.Listener;
using PayToPhone.Driver.App.Host.Infrastructure;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

internal class Program {
    private static async Task Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        var defaultSerializerSettings = new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = { new StringEnumConverter() }
        };

        JsonConvert.DefaultSettings = () => defaultSerializerSettings;

        // Add services to the container.
        builder.Services.AddControllers()
            .AddNewtonsoftJson(options => {
                options.SerializerSettings.Formatting = defaultSerializerSettings.Formatting;
                options.SerializerSettings.ContractResolver = defaultSerializerSettings.ContractResolver;
                foreach (var converter in defaultSerializerSettings.Converters) {
                    options.SerializerSettings.Converters.Add(converter);
                }
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // App +
        builder.Services.AddAppServices();
        // -----

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        var tabakonWebSocketServer = app.Services.GetRequiredService(typeof(ITabakonWebSocketServer)) as ITabakonWebSocketServer;
        tabakonWebSocketServer.Startlistener("http://*:5511/");

        var isMockIntegrator = false;
        //var isMockIntegrator = true;
        if (isMockIntegrator) {
            var payToPhoneIntegratorMock = app.Services.GetRequiredService(typeof(PayToPhoneIntegratorMock)) as PayToPhoneIntegratorMock;
            await payToPhoneIntegratorMock.Start();
        }

        await app.RunAsync();
    }
}