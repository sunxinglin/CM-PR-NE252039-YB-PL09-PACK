using MediatR;

namespace Automatic.Protocols.HeatingFilmPressurize.Models.Wraps
{
    public class DealReqPressurizeCompleteWraps : INotification
    {
        public int VectorCode { get; set; }
        public string PackCode { get; set; } = "";
        public string StartTime { get; set; } = "";
        public Dictionary<string, object> PressurizeDatas { get; set; } = new();
    }
}
