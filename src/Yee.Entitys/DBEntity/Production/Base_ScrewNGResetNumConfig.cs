using System.ComponentModel.DataAnnotations.Schema;

using Yee.Entitys.DBEntity.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.Production
{
    [Table("Base_ScrewNGResetConfig")]
    public class Base_ScrewNGResetConfig : BaseDataModel
    {
        /// <summary>
        /// 工序
        /// </summary>
        public int StepId { get; set; }
        public Base_Step Step { get; set; } = null!;

        /// <summary>
        /// 权限组
        /// </summary>
        public string RoleIds { get; set; } = null!;
        public int[] RoleIdArray => RoleIds.Split(',').Select(int.Parse).ToArray();

        /// <summary>
        /// 单颗螺丝可复位次数
        /// </summary>
        public int SingleScrewResetNum { get; set; } = 0;

    }
}
