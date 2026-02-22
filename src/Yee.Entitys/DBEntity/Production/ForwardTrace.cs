using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.Production
{
    public class ForwardTrace
    {
        /// <summary>
        /// PACK条码
        /// </summary>
        public string Code { get; set; }
        
        public List<Proc_StationTask_Main>? StationTask_Mains_Realtime { get; set; }
        
    }
}
