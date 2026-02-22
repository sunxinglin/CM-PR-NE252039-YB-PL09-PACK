using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DBEntity.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity
{
    /// <summary>
    /// 模组入箱数据收集信息表
    /// </summary>
    [Table("Proc_ModuleInBox_DataCollect")]
    public class Proc_ModuleInBox_DataCollect : BaseDataModel
    {
        public Proc_StationTask_Main? Proc_StationTask_Main { get; set; }
        public int? Proc_StationTask_MainId { get; set; }

        [MaxLength(50)]
        public string StationCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? PackCode { get; set; }

        public StationTaskStatusEnum Stauts { get; set; }

        public string ModuleInBoxDataJson { get; set; } = string.Empty;

        [NotMapped]
        public IList<ModuleInBoxDataJsonModel> ModuleInBoxDatas => JsonConvert.DeserializeObject<IList<ModuleInBoxDataJsonModel>>(ModuleInBoxDataJson)!;

    }

    public class ModuleInBoxDataJsonModel
    {

        public string? ParameterName { get; set; } = string.Empty;

        public ModuleInBoxDataTypeEnum ModuleInBoxDataType { get; set; }

        public string ModuleCode { get; set; } = string.Empty;

        public string ModulePN { get; set; } = string.Empty;

        public int Location { get; set; } = 1;

        public string DataValue { get; set; } = string.Empty;

        public string UpMesCode { get; set; } = string.Empty;

    }

}
