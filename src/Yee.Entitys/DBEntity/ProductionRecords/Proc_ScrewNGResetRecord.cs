using AsZero.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using Yee.Entitys.DBEntity.Common;
using Yee.Entitys.Production;

namespace Yee.Entitys.DBEntity.ProductionRecords
{
    [Table("Proc_ScrewNGResetRecord")]
    public class Proc_ScrewNGResetRecord : BaseDataModel
    {
        /// <summary>
        /// 用户
        /// </summary>
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        /// <summary>
        /// 工序
        /// </summary>
        public int StepId { get; set; }
        public Base_Step Step { get; set; } = null!;

        /// <summary>
        /// Pack码
        /// </summary>
        public string PackCode { get; set; } = null!;

        /// <summary>
        /// 螺丝序号
        /// </summary>
        public int ScrewSerialNo { get; set; } = 0;
    }

    internal class Proc_ScrewNGResetRecordConfiguration : IEntityTypeConfiguration<Proc_ScrewNGResetRecord>
    {
        public void Configure(EntityTypeBuilder<Proc_ScrewNGResetRecord> builder)
        {
            builder.HasIndex(e => e.PackCode);
        }
    }
}
