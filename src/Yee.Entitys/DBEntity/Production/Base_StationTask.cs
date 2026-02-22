using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.BaseData;
using Yee.Entitys.Common;

namespace Yee.Entitys.Production
{
    [Table("Base_StationTask")]
    public class Base_StationTask : CommonData
    {
        /// <summary>
        /// 任务编码
        /// </summary>
        [MaxLength(200)]
        public string? Code { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        [MaxLength(200)]
        public string? Name { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public StationTaskTypeEnum Type { get; set; }

        /// <summary>
        /// 是否有任务页面
        /// </summary>
        public bool HasPage { get; set; }
        /// <summary>
        /// 节拍
        /// </summary>
        public int? Clock { get; set; }
        /// <summary>
        /// 关联工位
        /// </summary>
        public Base_Step? Step { get; set; }
        public int StepId { get; set; }

        public int ProductId { get; set; }

        [NotMapped]
        public int StationID { get; set; }

        /// <summary>
        /// 任务序号
        /// </summary>·
        public int Sequence { get; set; }

        [NotMapped]
        /// <summary>
        /// 关联生产资源
        /// </summary>
        public List<Base_ProResource>? ListResource { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

    }
}
