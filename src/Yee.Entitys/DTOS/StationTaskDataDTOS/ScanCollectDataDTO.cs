using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class ScanCollectDataDTO :CommonDataDto
    {
        public Base_StationTaskScanCollect? ScanCollectData { get; set; }
        public List<StationTaskDTO>? StationTaskList { get; set; }
    }
}
