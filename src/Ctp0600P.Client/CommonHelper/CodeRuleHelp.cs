using System.Linq;

namespace Ctp0600P.Client.CommonHelper
{
    public static class CodeRuleHelp
    {
        /// <summary>
        /// 校验条码是否符合规则
        /// </summary>
        /// <param name="rule"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool CheckCodeRule(string rule,string code)
        {
            if (rule == null || code == null) { return false; }
            if (rule.Length!=code.Length)
            {
                return false;
            }
            var rulearry = rule.ToArray();
            var codearry = code.ToArray();
            for (int i = 0; i < rulearry.Length; i++)
            {
                if (Equals(rulearry[i], '*')  )
                {
                    continue;
                }
                else
                {
                    if (!Equals(rulearry[i], codearry[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
