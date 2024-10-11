using Application.Receiver.DI;
using RTSP.Receiver.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddApplicationService(builder.Configuration);
builder.Services.AddHostedService<ImageReceiverProcessor>();

var host = builder.Build();
host.Run();