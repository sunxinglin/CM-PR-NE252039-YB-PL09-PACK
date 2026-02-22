namespace Ctp0600P.Shared.CatlMes
{
    /// <summary>
    /// CATL MES 连接参数
    /// </summary>
    public class CatlMesConnectionParams
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Timeout { get; set; }
    }
}
