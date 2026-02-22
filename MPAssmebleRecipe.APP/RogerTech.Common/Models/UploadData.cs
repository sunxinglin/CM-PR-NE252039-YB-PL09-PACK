using SqlSugar;
using System;
using RogerTech.Tool;
using static log4net.Appender.RollingFileAppender;
using SqlSugar.DbConvert;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 本地数据存储实体类
    /// </summary>

    [SugarTable("UploadData")]

    public class UploadData
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        [SugarColumn(Length = 30)]
        public string SFC { get; set; }

        /// <summary>
        /// 上传接口
        /// </summary>
        [SugarColumn(Length = 50)]
        public string InterfaceName { get; set; }


        /// <summary>
        /// 工位名称
        /// </summary>
        [SugarColumn(Length = 20)]
        public string StationName { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        [SugarColumn(Length = 20)]
        public string UploadDataName { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>  
        [SugarColumn(ColumnDataType = "varchar(20)", SqlParameterDbType = typeof(EnumToStringConvert))]  
        public ParameterDataType? UploadDataType { get; set; }


        /// <summary>
        /// 参数值
        /// </summary>
        [SugarColumn(Length = 50)]
        public string UploadDataValue { get; set; }

        /// <summary>
        /// 是否重传
        /// </summary>
        [SugarColumn(ColumnDataType = "boolean")]
        public bool IsReupload { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [SugarColumn(DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime Time { get; set; }
    }
}


