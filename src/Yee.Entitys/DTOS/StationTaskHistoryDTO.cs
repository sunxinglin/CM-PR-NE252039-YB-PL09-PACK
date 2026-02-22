using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS
{
    public class StationTaskHistoryDTO
    {
        public Proc_StationTask_Main StationTaskMain { get; set; } = null!;
        public int MainId => StationTaskMain?.Id ?? 0;
        public bool IsNew { get; set; } = false;
        public IList<RecordHistoryDTO>? StationTaskRecords { get; set; } = new List<RecordHistoryDTO>();
    }

    public class RecordHistoryDTO
    {
        public Proc_StationTask_Record StationTaskRecord { get; set; } = null!;
        public int RecordId => StationTaskRecord?.Id ?? 0;
        public int StationTaskId => StationTaskRecord.Base_StationTaskId??0;
        public Proc_StationTask_ScanAccountCard? ScanAccountCards { get; set; }
        public IList<BomHistoryDTO>? Boms { get; set; }
        public IList<ScrewHistoryDTO>? Screws { get; set; }
        public Proc_StationTask_ScanCollect? ScanCollects { get; set; }
        public Proc_StationTask_UserInput? UserInputs { get; set; }
        public Proc_StationTask_CheckTimeout? CheckTimeouts { get; set; }
        public Proc_StationTask_TimeRecord? TimeRecords { get; set; }
        public Proc_StationTask_AnyLoad? AnyLoads { get; set; }
        public IList<Proc_StationTask_TightenRework>? TightenReworks { get; set; }

    }

    public class BomHistoryDTO
    {
        public Proc_StationTask_Bom StationTaskBom { get; set; } = null!;
        public int BomId => StationTaskBom?.Id ?? 0;
        public IList<Proc_StationTask_BomDetail>? BomDetails { get; set; }
    }

    public class ScrewHistoryDTO
    {
        public Proc_StationTask_BlotGun StationTaskScrew { get; set; } = null!;
        public int ScrewId => StationTaskScrew?.Id ?? 0;

        public IList<Proc_StationTask_BlotGunDetail>? ScrewDetails { get; set; }
    }
}