using WorkerService;
using WorkerService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<OnlyRecieveWorker>();
builder.Services.AddHostedService<SendReceiveWorker>();
builder.Services.AddHostedService<PresenceService>();
builder.Services.AddHostedService<SignalRWorker>();


var host = builder.Build();
host.Run();
