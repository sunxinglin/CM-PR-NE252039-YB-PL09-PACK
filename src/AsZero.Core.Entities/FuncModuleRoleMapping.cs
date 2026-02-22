using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AsZero.Core.Entities
{
    public class FuncModuleRoleMapping
    { 
        public int Id { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// 功能模块id
        /// </summary>
        public int FuncModuleId{ get; set; }
        /// <summary>
        /// 功能模块
        /// </summary>
        public FuncModule FuncModule { get; set; }
    }


    internal class FuncModuleRoleMappingEntityConfiguration : IEntityTypeConfiguration<FuncModuleRoleMapping>
    {
        public void Configure(EntityTypeBuilder<FuncModuleRoleMapping> builder)
        {
            builder.HasIndex(m => new { m.RoleName, m.FuncModuleId  });
        }
    }

}
