namespace Automatic.Client.Opts
{
    /// <summary>
    /// 软件的静态配置
    /// </summary>
    public class AppSetting
    {
        /// <summary>
        /// 应用标题
        /// </summary>
        public string AppTitle { get; set; } = "";

        /// <summary>
        /// 产线编码
        /// </summary>
        public string EquipId{ get; set; } = "";

        /// <summary>
        /// 视觉方案路径
        /// </summary>
        public string VisionSlnPath { get; set; } = "";

        /// <summary>
        /// 客户端端口
        /// </summary>
        public int ClientPort { get; set; }

    }
}
