using SqlSugar;

namespace RogerTech.Common.Models
{
    /// <summary>
    /// 应用配置实体
    /// </summary>
    [SugarTable("App_Config")]
    public class AppConfig
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 配置分类
        /// </summary>
        [SugarColumn(Length = 50)]
        public string Section { get; set; }

        /// <summary>
        /// 配置键
        /// </summary>
        [SugarColumn(Length = 50)]
        public string Key { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        [SugarColumn(Length = 200)]
        public string Value { get; set; }
    }
}