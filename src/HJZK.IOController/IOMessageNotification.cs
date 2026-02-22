using MediatR;

namespace HJZK.IOController
{
    public class IOMessageNotification : INotification
    {
        public IOMessageNotification()
        {

        }
        public string Name { get; set; }
        public IOStatues IOStatues { get; set; }

        public bool[] Di { get; set; } = new bool[16];
        public bool[] Do { get; set; } = new bool[16];
        public IOBoxConfig IOBoxConfig { get;  set; }

        public IList<DIStatus> DIStatus { get; set; } 
    }

    public class DIStatus
    {
        public string ControlName { get; set; } = "";
        public bool Status { get; set; }
    }
}
