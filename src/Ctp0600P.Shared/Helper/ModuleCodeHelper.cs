namespace Ctp0600P.Shared.Helper
{
    public class ModuleCodeHelper
    {
        public static bool DecodeMoudleCode(string 入缓存模组编号, out int moudleType)
        {
            try
            {
                if (string.IsNullOrEmpty(入缓存模组编号))
                {
                    moudleType = 0;
                    return false;
                }
                if (入缓存模组编号.Length != 24)
                {
                    moudleType = 0;
                    return false;
                }
                if (入缓存模组编号.Substring(6, 1) != "M")
                {
                    moudleType = 0;
                    return false;
                }
                moudleType = int.Parse(入缓存模组编号.Substring(7, 1));
                return true;
            }
            catch
            {
                moudleType = 0;
                return false;
            }
        }
    }
}
