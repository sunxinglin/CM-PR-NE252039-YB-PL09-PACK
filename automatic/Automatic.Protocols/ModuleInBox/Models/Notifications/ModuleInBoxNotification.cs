using Automatic.Protocols.ModuleInBox.Models.Datas;
using MediatR;

namespace Automatic.Protocols.ModuleInBox.Models.Notifications
{
    public class ModuleInBoxNotification : INotification
    {
        public IList<ModuleInBoxData> ModuleInBoxDatas { get; set; }
    }
}
