using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.CommonEntity;
using Yee.Entitys.DBEntity;
using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS
{
    public  class StationPack_AGV_Task_Record_DTO
    {
        public Proc_AGVStatus? Proc_AGVStatus { get; set; }
        public StationConfig? StationConfig { get; set; }
        public StaionHisDataDTO? Pack_His_Data { get; set; }
        
    }
}
