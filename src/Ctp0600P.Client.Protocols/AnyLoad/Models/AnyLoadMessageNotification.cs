using MediatR;

namespace Ctp0600P.Client.Protocols.AnyLoad
{
    public class AnyLoadMessageNotification : INotification
    {
        public AnyLoadMessageNotification()
        {

        }

        public string Message { get; set; }
    }
}
