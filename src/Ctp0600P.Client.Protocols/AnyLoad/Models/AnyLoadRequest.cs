using MediatR;

namespace Ctp0600P.Client.Protocols
{
    public class AnyLoadRequest : INotification 
    {
        public string AnyLoadContext { get; set; } = "";
        public string AnyLoadPortName { get; set; } = "";
    }
}
