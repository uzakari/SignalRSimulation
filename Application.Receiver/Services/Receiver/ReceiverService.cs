using System.Drawing;

namespace Application.Receiver.Services.Receiver;

public class ReceiverService : IReceiverService
{
    public Task ProcessRtspStringToImage(string base64String)
    {
        var frameBytes = Convert.FromBase64String(base64String);

        using (MemoryStream ms = new MemoryStream(frameBytes))
        {
            using (Bitmap image = new Bitmap(ms))
            {
                var filePath = Path.Combine("ReceivedImages", $"frame_{DateTime.Now.Ticks}.jpg");
                Directory.CreateDirectory("ReceivedImages");
                image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                Console.WriteLine($"Image saved: {filePath}");
            }
        }
        return Task.CompletedTask;
    }
}