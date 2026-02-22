using Automatic.Entity.DataDtos;
using MediatR;

namespace Automatic.Protocols.UpperCoverTighten2.Models.Wraps
{
    public class DealReqTightenCompleteWraps : INotification
    {
        public int VectorCode { get; set; }
        public string PackCode { get; set; }
        public IList<AutoTightenDataUploadDto.AutoTightenDataUploadTightenItem> TightenDatas { get; set; }
    }
}
