using MQTTnet.Server;
using MQTTnet.AspNetCore.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var optionsBuilderMQTT = new MqttServerOptionsBuilder()
                             .WithApplicationMessageInterceptor(context => {
                                if (context.ApplicationMessage.Topic == "teste/topico2")
                                {
                                    string inPayload = Encoding.UTF8.GetString(context.ApplicationMessage.Payload, 0, context.ApplicationMessage.Payload.Length);

                                    Console.WriteLine("Mensagem recebida: {0}", inPayload);
                                }
                             });

builder.Services.AddHostedMqttServer(optionsBuilderMQTT.Build())
                .AddMqttConnectionHandler()
                .AddConnections();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.WebHost.ConfigureKestrel(options => {
    options.ListenAnyIP(1883,l => l.UseMqtt());
    options.ListenAnyIP(5000);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMqttServer(async server => {});

app.Run();