
using Application.Receiver.Services.Receiver;
using Domain.Settings;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace RTSP.Receiver.BackgroundServices;

public class ImageReceiverProcessor : BackgroundService
{
    private readonly ILogger<ImageReceiverProcessor> _logger;
    private readonly IServiceProvider _serviceProvider;
    private HubConnection _hubConnection;
    private readonly SignalRClientSettings _signalRClientSettings;

    public ImageReceiverProcessor(ILogger<ImageReceiverProcessor> logger,
        IOptions<SignalRClientSettings> options,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _signalRClientSettings = options.Value;
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_signalRClientSettings.BaseUrl)
            .Build();      
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _hubConnection.On<string>("ReceiveFrame",  async (frameBase64) =>
        {
            using var scope = _serviceProvider.CreateScope();
            var imageProcessorService = scope.ServiceProvider.GetRequiredService<IReceiverService>();
            await imageProcessorService.ProcessRtspStringToImage(frameBase64);
        });

        try
        {
            await _hubConnection.StartAsync(stoppingToken);
            _logger.LogInformation("SignalR connection started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting SignalR connection");
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("ImageReceiverProcessor running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
        await _hubConnection.StopAsync(stoppingToken);
    }
}