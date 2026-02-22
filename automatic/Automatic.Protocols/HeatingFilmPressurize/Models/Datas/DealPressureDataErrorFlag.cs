using Automatic.Entity;
using Automatic.Protocols.Common;

namespace Automatic.Protocols.HeatingFilmPressurize.Models.Datas
{
    public enum DealPressureDataErrorFlag : ushort
    {
        None = 0,
        PCError = 1 << 0,
        DataError = 1 << 1,
        CatlMesError = 1 << 2,
        FlowError = 1 << 3,
        CheckAGVBindCodeError = 1 << 4,
        SameValueError = 1 << 5,
    }

    public class DealPressureDataErrorFlagBuilder : FlagsBuilder<DealPressureDataErrorFlag>
    {
        public DealPressureDataErrorFlagBuilder(DealPressureDataErrorFlag wCmd) : base(wCmd)
        {
        }

        public DealPressureDataErrorFlagBuilder SetAlarm(string ErrorType)
        {
            SetOnOff(DealPressureDataErrorFlag.DataError, ErrorType == ReponseErrorType.数据异常)
                .SetOnOff(DealPressureDataErrorFlag.CatlMesError, ErrorType == ReponseErrorType.CatlMes错误)
                .SetOnOff(DealPressureDataErrorFlag.PCError, ErrorType == ReponseErrorType.上位机错误)
                .SetOnOff(DealPressureDataErrorFlag.FlowError, ErrorType == ReponseErrorType.流程顺序错误)
                .SetOnOff(DealPressureDataErrorFlag.SameValueError, ErrorType == ReponseErrorType.同值报警)
                .SetOnOff(DealPressureDataErrorFlag.CheckAGVBindCodeError, ErrorType == ReponseErrorType.载具绑定错误)
                .Build();

            return this;
        }

        public DealPressureDataErrorFlagBuilder ResetAllAlarm()
        {
            SetOnOff(DealPressureDataErrorFlag.DataError, false)
                .SetOnOff(DealPressureDataErrorFlag.CatlMesError, false)
                .SetOnOff(DealPressureDataErrorFlag.PCError, false)
                .SetOnOff(DealPressureDataErrorFlag.FlowError, false)
                .SetOnOff(DealPressureDataErrorFlag.SameValueError, false)
                .SetOnOff(DealPressureDataErrorFlag.CheckAGVBindCodeError, false)
                .Build();
            return this;
        }
    }
}
