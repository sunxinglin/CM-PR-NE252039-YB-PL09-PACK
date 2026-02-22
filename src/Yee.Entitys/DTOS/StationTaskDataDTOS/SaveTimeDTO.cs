using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class SaveTimeDTO :CommonDataDto
    {
        public string PackCode { get; set; } = "";

        public string StationCode { get; set; } = "";

        public string RecordTimeStr { get; set; } = "";

        public string TimeFlag { get; set; } = "";  

        public string UploadMesCode { get; set; } = "";

        public string TaskName { get; set; } = "";
    }
}
