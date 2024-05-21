using WorkerService;
using WorkerService.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<SecondWorker>();
builder.Services.AddHostedService<PresenceService>();
builder.Services.AddHostedService<SignalRWorker>();


var host = builder.Build();
host.Run();
