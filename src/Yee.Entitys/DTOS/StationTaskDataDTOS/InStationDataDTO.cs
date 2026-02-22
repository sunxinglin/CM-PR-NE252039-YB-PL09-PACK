using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DTOS.StationTaskDataDTOS
{
    public class InStationDataDTO
    {
        public string AGVNo { get; set; }
        public string PackCode { get; set; }
        public int StepId { get; set; }
        public int StationId { get; set; }
        public List<StationTaskDTO> StationTaskList { get; set; }= new List<StationTaskDTO>();
    }
}
