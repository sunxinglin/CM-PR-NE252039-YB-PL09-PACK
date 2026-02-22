using MediatR;

namespace Ctp0600P.Client.Protocols.ScanCode.Models
{
    public class ScanCodeGunRequest : INotification//IRequest<ScanCodeGunResponse>
    {
        public string ScanCodeContext { get; set; } = "";
        public string ScanCodePortName { get; set; } = "";
        public bool FromRework { get; set; } = false;
    }
}
