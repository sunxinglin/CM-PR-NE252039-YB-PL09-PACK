using MediatR;

namespace Ctp0600P.Client.Protocols;

public class ScanMessageNotification : INotification
{
    public ScanMessageNotification()
    {

    }

    public string Message { get; set; }
}