using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yee.Entitys.AlarmMgmt;

[Table("Alarm")]
public class Alarm
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public AlarmCode Code { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string? StationCode { get; set; } = string.Empty;
    public string? PackCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public bool IsFinish { get; set; } = false;
    public DateTime OccurTime { get; set; }
    public DateTime ClearDate { get; set; }
    public string TypeName => Code.ToString();

    [NotMapped]
    public string DeviceNo { get; set; } = string.Empty;


    [NotMapped]
    public int CodeValue => (int)Code;

    /// <summary>
    /// 拧紧枪NG信息
    /// </summary>
    [NotMapped]
    public AlarmExtra.TightenNG? TightenNGExtra { get; set; }
}

public class AlarmExtra
{
    public class TightenNG
    {
        public string? DeviceNo { get; set; }
        public int ScrewSerialNo { get; set; }
    }
}

public class AlarmData
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Module { get; set; }
}

public enum AlarmCode
{
    IO连接失败 = 1001,
    拧紧枪连接失败 = 1002,
    电子秤连接失败 = 1003,

    系统运行错误 = 2001,
    防呆报警 = 2002,
    超时检测报警 = 2003,
    静置时长不足报警 = 2004,

    扫码枪错误 = 3001,
    装配错误 = 3002,
    条码规则比对错误 = 3003,

    拧紧枪错误 = 4001,
    拧紧NG = 4002,
    拧紧使能NG = 4003,
    多次拧紧NG = 4004,
    防重打NG = 4005,
    空转NG = 4006,


    称重错误 = 5001,

    AGV错误 = 6001,

    CatlMES错误 = 7001,


    电源总开关反馈 = 9001,
    电源24V开关反馈1 = 9002,
    照明电源空开反馈 = 9003,
    电源24V空开反馈2 = 9004,
    散热风扇反馈 = 9005,

    用户输入错误 = 10001,
    扫码输入错误 = 10002,

    时间记录错误 = 11001,
    拉铆错误 = 11002,
        
    返工错误 = 12001,

}

public static class AlarmName
{
    public const string IO_DISCONNECT = "IO_DISCONNECT";
    public const string DESOUTTER_DISCONNECT = "拧紧枪";
    public const string 电源总开关反馈 = "电源总开关故障";
    public const string 电源24V开关反馈1 = "电源24V开关1故障";
    public const string 照明电源空开反馈 = "照明电源空开故障";
    public const string 电源24V空开反馈2 = "电源24V空开2故障";
    public const string 与服务器断开连接 = "与服务器断开连接";
}

public static class AlarmDescription
{
    public const string IO_DISCONNECT = "无法连接IO模块";
    public const string DESOUTTER_DISCONNECT = "无法连接拧紧枪";
    public const string 电源总开关反馈 = "电源总开关故障，请检查电源总开关";
    public const string 电源24V开关反馈1 = "电源24V1开关故障，请检查电源24V1开关";
    public const string 照明电源空开反馈 = "照明电源空开故障，请检查照明电源空开";
    public const string 电源24V开关反馈2 = "电源24V2开关故障，请检查电源24V2开关";
    public const string 与服务器断开连接  = "与服务器断开连接,5s后重新连接";
}

public static class AlarmModule
{
    public const string IO_MODULE = "MoxaIO";
    public const string DESOUTTER_MODULE = "Desoutter";
    public const string SCAN_CODE_GUN_MODULE = "ScanCodeGun";
    public const string ELECTRONIC_SCALE = "ElectronicScale";
    public const string SERVER_MODULE = "Server";
    public const string AGV_MODULE = "AGV";
}