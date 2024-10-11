namespace Application.Receiver.Services.Receiver;

public interface IReceiverService
{
    Task ProcessRtspStringToImage(string base64String);
}