namespace RogerTech.Share
{
    public class ErrorCode
    {
        public enum AutomaticLabelingStation
        {
            无报错              = 0,
            报警代码初始化值    = -1,
            传值为空            = -11,
            条码重码            = -17,
            条码不匹配          = -18,
            等级码制保存失败    = -19,

            数值超限            = -40,
            数值格式错误        = -41,
            变量读取异常        = -42,

            通用异常报错        = -99
        }
    }
}
