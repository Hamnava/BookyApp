using WorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<SecondWorker>();
builder.Services.AddHostedService<SignalRWorker>();

var host = builder.Build();
host.Run();
