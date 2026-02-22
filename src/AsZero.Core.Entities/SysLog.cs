using FutureTech.Dal.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsZero.Core.Entities
{
    [Table("SysLog")]
    public class SysLog : FutureBaseEntity<int>
    {
        public Sys_LogType LogType { get; set; }
        public string Message { get; set; }
        public DateTime CreateTime { get; set; }
        public string TypeName { get { return LogType.ToString(); } }

        public string Operator { get; set; }
    }

    public enum Sys_LogType
    {

        用户登录,

        新增用户,
        修改用户,


        新增角色,
        修改角色,
        删除角色,
        角色配置,
        权限配置,

        新增产品,
        修改产品,
        删除产品,

        新增工序,
        修改工序,
        删除工序,

        新增工艺,
        修改工艺,
        删除工艺,

        新增配方,
        修改配方,
        删除配方,

        新增工位,
        修改工位,
        删除工位,

        新增生产资源,
        修改生产资源,
        删除生产资源,

        新增AGV,
        删除AGV,
        绑定AGV,
        解绑AGV,

        自动站数据重传,
        强制完工,
        强制未完工,


        踢料,
        导入配方,
        首件数据重传,

        清除Bom数据,
        清除收数数据
    }
}
