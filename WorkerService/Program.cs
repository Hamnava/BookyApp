using WorkerService.Models;
using WorkerService.Services;
using WorkerService.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<OnlyRecieveWorker>();
builder.Services.AddHostedService<SendReceiveWorker>();
builder.Services.AddHostedService<PresenceService>();
builder.Services.AddHostedService<SignalRWorker>();

// Bind the model to appsettings.json data
builder.Services.Configure<SignalRConfiguration>(builder.Configuration.GetSection("SignalRConfiguration"));


var host = builder.Build();

host.Run();
