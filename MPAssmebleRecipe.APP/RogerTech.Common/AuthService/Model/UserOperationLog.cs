using System;
using SqlSugar;

namespace RogerTech.Common.AuthService.Model
{
    [SugarTable("u_operation_log")]
    public class UserOperationLog
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string UserName { get; set; }        //名字
            
        public string EmployeeId { get; set; }      //工号

        public Operation Operation { get; set; }   //操作

        public string ViewName { get; set; }        //操作页面

        [SugarColumn(ColumnDataType = "text")]
        public string Description { get; set; }     //具体内容

        [SugarColumn(IsNullable = true)]
        public string UserWorkstations { get; set; }       //工位   之后需要进行开放

        [SugarColumn(IsNullable = true)]
        public string IpAddress { get; set; }       //Ip   之后需要进行开放

        [SugarColumn(IsNullable = true)]
        public Device DeviceInfo { get; set; }      //设备 之后需要进行开放

        [SugarColumn(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime CreatTime { get; set; }     //创建时间
    }
    public enum Operation
    {
        Login,
        OutLogin,
        CloseApp,
        Add,
        Update,
        Delet,
        Select
    }
    public enum Device
    {
        客户端,
        手机,
        平板
    }
}
