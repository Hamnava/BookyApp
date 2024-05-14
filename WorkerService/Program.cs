using WorkerService;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<SecondWorker>();
builder.Services.AddHostedService<SignalRWorker>();

// Register HeartbeatSender as a hosted service
builder.Services.AddHostedService<HeartbeatSender>();


var host = builder.Build();
host.Run();
