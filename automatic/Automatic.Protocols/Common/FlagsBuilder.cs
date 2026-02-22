using System.Runtime.CompilerServices;

namespace Automatic.Protocols.Common
{
    /// <summary>
    /// flags (max 4 bytes)
    /// </summary>
    /// <typeparam name="TFlags"></typeparam>
    public class FlagsBuilder<TFlags>
        where TFlags : Enum
    {
        protected uint _wCmd;

        public FlagsBuilder(TFlags wCmd)
        {
            _wCmd = Unsafe.As<TFlags, uint>(ref wCmd);
        }

        /// <summary>
        /// 构建命令字
        /// </summary>
        /// <returns></returns>
        public virtual TFlags Build() => Unsafe.As<uint, TFlags>(ref _wCmd);

        public virtual FlagsBuilder<TFlags> SetOnOff(TFlags bitIndicator, bool onoff)
        {
            var indicator = Unsafe.As<TFlags, uint>(ref bitIndicator);
            _wCmd = onoff ? _wCmd | indicator : _wCmd & ~indicator;
            return this;
        }

        public virtual FlagsBuilder<TFlags> RestAll()
        {
            _wCmd = 0;
            return this;
        }
    }
}
