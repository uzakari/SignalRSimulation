using AForge.Video;
using Application.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;

namespace Application.Features.Commands;

public record CaptureFrameCommand(string RstpUrl) : IRequest<Unit>;

public class CaptureFrameCommandHandler : IRequestHandler<CaptureFrameCommand, Unit>
{
    private readonly IHubContext<FrameHub> _hubContext;
    private static string Base64ToSend { get; set; } = string.Empty;
    
    public CaptureFrameCommandHandler(IHubContext<FrameHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public Task<Unit> Handle(CaptureFrameCommand request, CancellationToken cancellationToken)
    {
        var stream = new MJPEGStream(request.RstpUrl);
        stream.NewFrame += new NewFrameEventHandler(CaptureFrame);
        stream.Start();
        
        return Task.FromResult(Unit.Value);
    }
    
    private async void CaptureFrame(object sender, NewFrameEventArgs eventArgs)
    { 
        using var bitmap = (System.Drawing.Bitmap)eventArgs.Frame.Clone();
        using var image = ConvertToImageSharpImage(bitmap);
        using var ms = new MemoryStream();
        image.Save(ms, new JpegEncoder());
        var frameBytes = ms.ToArray();
        var frameBase64 = Convert.ToBase64String(frameBytes);
        
        await _hubContext.Clients.All.SendAsync("ReceiveFrame", frameBase64);
    }
    private static Image<Rgba32> ConvertToImageSharpImage(System.Drawing.Bitmap bitmap)
    {
        using var ms = new MemoryStream();
        bitmap?.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp); // Save as BMP to preserve quality
        ms.Seek(0, SeekOrigin.Begin);
        return Image.Load<Rgba32>(ms);
    }
    
}