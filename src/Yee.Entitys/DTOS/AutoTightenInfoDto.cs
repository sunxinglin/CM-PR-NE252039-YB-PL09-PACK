namespace Yee.Entitys.DTOS;

public class AutoTightenInfoDto
{
    public int Limit { get; set; } = 20;
    public int Page { get; set; } = 1;
    public string? PackCode { get; set; }
        
    public DateTime? BeginTime { get; set; }
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 自动拧紧的工站类型：
    ///     0：模组
    ///     1：上盖
    ///     2：压条
    /// </summary>
    public int? TightenType { get; set; }
}

public class ExternalAutoTightenDataListItemDto
{
    public int Id { get; set; }
    public string Sfc { get; set; } = string.Empty;
    public string StationName { get; set; } = string.Empty;
    public int TightenType { get; set; }
    public DateTime? CreateTime { get; set; }
    public DateTime? UpdateTime { get; set; }

    public int TotalCount { get; set; }
    public int OkCount { get; set; }
    public int NgCount { get; set; }
}
