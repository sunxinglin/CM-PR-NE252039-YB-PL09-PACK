using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver.MidMessage
{
    public class DataField_MID0061
    {
        public DataField_MID0061(byte[] message)
        {
            encoding = new ASCIIEncoding();
            this.Create(message);
        }
        public string _01_CellID { get; set; } = string.Empty;
        public string _02_ChannelID { get; set; } = string.Empty;
        public string _03_TorqueControllerName { get; set; } = string.Empty;
        public string _04_VINNumber { get; set; } = string.Empty;
        public string _05_JobID { get; set; } = string.Empty;
        public string _06_ParameterSetNumber { get; set; } = string.Empty;
        public string _07_Strategy { get; set; } = string.Empty;
        public string _08_StrategyOptions { get; set; } = string.Empty;
        public string _09_BatchSize { get; set; } = string.Empty;
        public string _10_BatchCounter { get; set; } = string.Empty;

        /// <summary>
        /// 总是否合格
        /// </summary>
        public string _11_TighteningStatus { get; set; } = string.Empty;
        public string _12_BatchStatus { get; set; } = string.Empty;

        /// <summary>
        /// 扭矩否合格
        /// </summary>
        public string _13_TorqueStatus { get; set; } = string.Empty;

        /// <summary>
        /// 角度是否合格
        /// </summary>
        public string _14_AngleStatus { get; set; } = string.Empty;

        /// <summary>
        /// 扭矩工艺下限
        /// </summary>
        public string _21_TorqueMinLimit { get; set; } = string.Empty;//扭矩工艺下限
        /// <summary>
        /// 扭矩工艺上限
        /// </summary>
        public string _22_TorqueMaxLimit { get; set; } = string.Empty;//扭矩工艺上限

        /// <summary>
        /// 目标扭矩值
        /// </summary>
        public string _23_TorqueFinalTarget { get; set; } = string.Empty;//
        /// <summary>
        /// 扭矩结果
        /// </summary>
        public string _24_Torque { get; set; } = string.Empty;//扭矩结果
        /// <summary>
        /// 工艺角度下限
        /// </summary>
        public string _25_AngleMin { get; set; } = string.Empty;//工艺角度下限
        /// <summary>
        /// 工艺角度上限
        /// </summary>
        public string _26_AngleMax { get; set; } = string.Empty;//工艺角度上限
        /// <summary>
        /// 目标角度值
        /// </summary>
        public string _27_FinalAngleTarget { get; set; } = string.Empty;//
        /// <summary>
        /// 角度结果
        /// </summary>
        public string _28_Angle { get; set; } = string.Empty;//角度结果

        public string _15_RundownAngleStatus { get; set; } = string.Empty;
        public string _16_CurrentMonitoringStatus { get; set; } = string.Empty;
        public string _17_SelftapStatus { get; set; } = string.Empty;
        public string _18_PrevailTorqueMonitoringStatus { get; set; } = string.Empty;
        public string _19_PrevailTorqueCompensateStatus { get; set; } = string.Empty;
        public string _20_TighteningErrorStatus { get; set; } = string.Empty;

        public string _29_RundownAngleMin { get; set; } = string.Empty;
        public string _30_RundownAngleMax { get; set; } = string.Empty;
        public string _31_RundownAngle { get; set; } = string.Empty;
        public string _32_CurrentMonitoringMin { get; set; } = string.Empty;
        public string _33_CurrentMonitoringMax { get; set; } = string.Empty;
        public string _34_CurrentMonitoringValue { get; set; } = string.Empty;
        public string _35_SelftapMin { get; set; } = string.Empty;
        public string _36_SelftapMax { get; set; } = string.Empty;
        public string _37_SelftapTorque { get; set; } = string.Empty;
        public string _38_PrevailTorqueMonitoringMin { get; set; } = string.Empty;
        public string _39_PrevailTorqueMonitoringMax { get; set; } = string.Empty;
        public string _40_PrevailTorque { get; set; } = string.Empty;
        public string _41_TighteningID { get; set; } = string.Empty;
        public string _42_JobSequenceNumber { get; set; } = string.Empty;
        public string _43_SyncTighteningID { get; set; } = string.Empty;
        public string _44_SyncTighteningID { get; set; } = string.Empty;
        public string _45_TimeStamp { get; set; } = string.Empty;
        public string _46_DatetimeOfLastChangeInParameterSetSettings { get; set; } = string.Empty;
        public string _47_ParameterSetName { get; set; } = string.Empty;
        public string _48_TorqueValuesUnit { get; set; } = string.Empty;
        public string _49_ResultType { get; set; } = string.Empty;
        public string _50_IdentifierResultPart2 { get; set; } = string.Empty;
        public string _51_IdentifierResultPart3 { get; set; } = string.Empty;
        public string _52_IdentifierResultPart4 { get; set; } = string.Empty;
        public string _53_CustomerTighteningErrorCode { get; set; } = string.Empty;
        public string _54_PrevailTorqueCompensateValue { get; set; } = string.Empty;
        public string _55_TighteningErrorStatus2 { get; set; } = string.Empty;
        public string _56_CompensatedAngle { get; set; } = string.Empty;

        /// <summary>
        /// 带小数点的角度结果
        /// </summary>
        public string _57_FinalAngleDecimal { get; set; } = string.Empty;//带小数点的角度结果

        public ASCIIEncoding encoding { get; set; }

        private void Create(byte[] message)
        {
            this.Get01_CellID(message);
            this.Get02_ChannelID(message);
            this.Get03_TorqueControllerName(message);
            this.Get04_VINNumber(message);
            this.Get05_JobID(message);
            this.Get06_ParameterSetNumber(message);
            this.Get07_Strategy(message);
            this.Get08_StrategyOptions(message);
            this.Get09_BatchSize(message);
            this.Get10_BatchCounter(message);
            this.Get11_TighteningStatus(message);
            this.Get12_BatchStatus(message);
            this.Get13_TorqueStatus(message);//总是否合格
            this.Get14_AngleStatus(message);//角度是否合格
            this.Get15_RundownAngleStatus(message);
            this.Get16_CurrentMonitoringStatus(message);
            this.Get17_SelftapStatus(message);
            this.Get18_PrevailTorqueMonitoringStatus(message);
            this.Get19_PrevailTorqueCompensateStatus(message);
            this.Get20_TighteningErrorStatus(message);
            this.Get21_TorqueMinLimit(message);//扭矩工艺下限
            this.Get22_TorqueMaxLimit(message);//扭矩工艺上限
            this.Get23_TorqueFinalTarget(message);//工艺扭矩值
            this.Get24_Torque(message);//扭矩结果
            this.Get25_AngleMin(message);//工艺角度下限
            this.Get26_AngleMax(message);//工艺角度上限
            this.Get27_FinalAngleTarget(message);//工艺角度值
            this.Get28_Angle(message);//角度结果
            this.Get29_RundownAngleMin(message);
            this.Get30_RundownAngleMax(message);
            this.Get31_RundownAngle(message);
            this.Get32_CurrentMonitoringMin(message);
            this.Get33_CurrentMonitoringMax(message);
            this.Get34_CurrentMonitoringValue(message);
            this.Get35_SelftapMin(message);
            this.Get36_SelftapMax(message);
            this.Get37_SelftapTorque(message);
            this.Get38_PrevailTorqueMonitoringMin(message);
            this.Get39_PrevailTorqueMonitoringMax(message);
            this.Get40_PrevailTorque(message);
            this.Get41_TighteningID(message);
            this.Get42_JobSequenceNumber(message);
            this.Get43_SyncTighteningID(message);
            this.Get44_SyncTighteningID(message);
            this.Get45_TimeStamp(message);
            this.Get46_DatetimeOfLastChangeInParameterSetSettings(message);
            this.Get47_ParameterSetName(message);
            this.Get48_TorqueValuesUnit(message);
            this.Get49_ResultType(message);
            this.Get50_IdentifierResultPart2(message);
            this.Get51_IdentifierResultPart3(message);
            this.Get52_IdentifierResultPart4(message);
            this.Get53_CustomerTighteningErrorCode(message);
            this.Get54_PrevailTorqueCompensateValue(message);
            this.Get55_TighteningErrorStatus2(message);
            this.Get56_CompensatedAngle(message);
            this.Get57_FinalAngleDecimal(message);//带小数点的角度结果
        }

        public void Get01_CellID(byte[] message)
        {
            string seq = encoding.GetString(message, 20, 2);
            string data = encoding.GetString(message, 22, 4);
            if (seq == "01")
            {
                _01_CellID = data;
            }
        }

        public void Get02_ChannelID(byte[] message)
        {
            string seq = encoding.GetString(message, 26, 2);
            string data = encoding.GetString(message, 28, 2);
            if (seq == "02")
            {
                _02_ChannelID = data;
            }
        }

        public void Get03_TorqueControllerName(byte[] message)
        {
            string seq = encoding.GetString(message, 30, 2);
            string data = encoding.GetString(message, 32, 25);
            if (seq == "03")
            {
                _03_TorqueControllerName = data;
            }
        }

        public void Get04_VINNumber(byte[] message)
        {
            string seq = encoding.GetString(message, 57, 2);
            string data = encoding.GetString(message, 59, 25);
            if (seq == "04")
            {
                _04_VINNumber = data;
            }
        }

        public void Get05_JobID(byte[] message)
        {
            string seq = encoding.GetString(message, 84, 2);
            string data = encoding.GetString(message, 86, 4);
            if (seq == "05")
            {
                _05_JobID = data;
            }
        }

        public void Get06_ParameterSetNumber(byte[] message)
        {
            string seq = encoding.GetString(message, 90, 2);
            string data = encoding.GetString(message, 92, 3);
            if (seq == "06")
            {
                _06_ParameterSetNumber = data;
            }
        }
        public void Get07_Strategy(byte[] message)
        {
            string seq = encoding.GetString(message, 95, 2);
            string data = encoding.GetString(message, 97, 2);
            if (seq == "07")
            {
                _07_Strategy = data;
            }
        }
        public void Get08_StrategyOptions(byte[] message)
        {
            string seq = encoding.GetString(message, 99, 2);
            string data = encoding.GetString(message, 101, 5);
            if (seq == "08")
            {
                _08_StrategyOptions = data;
            }
        }

        public void Get09_BatchSize(byte[] message)
        {
            string seq = encoding.GetString(message, 106, 2);
            string data = encoding.GetString(message, 108, 4);
            if (seq == "09")
            {
                _09_BatchSize = data;
            }

        }
        public void Get10_BatchCounter(byte[] message)
        {
            string seq = encoding.GetString(message, 112, 2);
            string data = encoding.GetString(message, 114, 4);
            if (seq == "10")
            {
                _10_BatchCounter = data;
            }
        }

        public void Get11_TighteningStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 118, 2);
            string data = encoding.GetString(message, 120, 1);
            if (seq == "11")
            {
                _11_TighteningStatus = data;
            }
        }
        public void Get12_BatchStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 121, 2);
            string data = encoding.GetString(message, 123, 1);
            if (seq == "12")
            {
                _12_BatchStatus = data;
            }
        }
        public void Get13_TorqueStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 124, 2);
            string data = encoding.GetString(message, 126, 1);
            if (seq == "13")
            {
                _13_TorqueStatus = data;//总是否合格
            }
        }
        public void Get14_AngleStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 127, 2);
            string data = encoding.GetString(message, 129, 1);
            if (seq == "14")
            {
                _14_AngleStatus = data;//角度是否合格
            }
        }
        public void Get15_RundownAngleStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 130, 2);
            string data = encoding.GetString(message, 132, 1);
            if (seq == "15")
            {
                _15_RundownAngleStatus = data;
            }
        }
        public void Get16_CurrentMonitoringStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 133, 2);
            string data = encoding.GetString(message, 135, 1);
            if (seq == "16")
            {
                _16_CurrentMonitoringStatus = data;
            }
        }
        public void Get17_SelftapStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 136, 2);
            string data = encoding.GetString(message, 138, 1);
            if (seq == "17")
            {
                _17_SelftapStatus = data;
            }
        }
        public void Get18_PrevailTorqueMonitoringStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 139, 2);
            string data = encoding.GetString(message, 141, 1);
            if (seq == "18")
            {
                _18_PrevailTorqueMonitoringStatus = data;
            }
        }
        public void Get19_PrevailTorqueCompensateStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 142, 2);
            string data = encoding.GetString(message, 144, 1);
            if (seq == "19")
            {
                _19_PrevailTorqueCompensateStatus = data;
            }
        }
        public void Get20_TighteningErrorStatus(byte[] message)
        {
            string seq = encoding.GetString(message, 145, 2);
            string data = encoding.GetString(message, 147, 10);
            if (seq == "20")
            {
                _20_TighteningErrorStatus = data;
            }
        }
        public void Get21_TorqueMinLimit(byte[] message)
        {
            string seq = encoding.GetString(message, 157, 2);
            string data = encoding.GetString(message, 159, 6);
            if (seq == "21")
            {
                _21_TorqueMinLimit = data;
            }
        }
        public void Get22_TorqueMaxLimit(byte[] message)
        {
            string seq = encoding.GetString(message, 165, 2);
            string data = encoding.GetString(message, 167, 6);
            if (seq == "22")
            {
                _22_TorqueMaxLimit = data;
            }
        }
        public void Get23_TorqueFinalTarget(byte[] message)
        {
            string seq = encoding.GetString(message, 173, 2);
            string data = encoding.GetString(message, 175, 6);
            if (seq == "23")
            {
                _23_TorqueFinalTarget = data;
            }
        }
        public void Get24_Torque(byte[] message)
        {
            string seq = encoding.GetString(message, 181, 2);
            string data = encoding.GetString(message, 183, 6);
            if (seq == "24")
            {
                _24_Torque = data;
            }
        }
        public void Get25_AngleMin(byte[] message)
        {
            string seq = encoding.GetString(message, 189, 2);
            string data = encoding.GetString(message, 191, 5);
            if (seq == "25")
            {
                _25_AngleMin = data;
            }
        }
        public void Get26_AngleMax(byte[] message)
        {
            string seq = encoding.GetString(message, 196, 2);
            string data = encoding.GetString(message, 198, 5);
            if (seq == "26")
            {
                _26_AngleMax = data;
            }
        }
        public void Get27_FinalAngleTarget(byte[] message)
        {
            string seq = encoding.GetString(message, 203, 2);
            string data = encoding.GetString(message, 205, 5);
            if (seq == "27")
            {
                _27_FinalAngleTarget = data;
            }
        }
        public void Get28_Angle(byte[] message)
        {
            string seq = encoding.GetString(message, 210, 2);
            string data = encoding.GetString(message, 212, 5);
            if (seq == "28")
            {
                _28_Angle = data;
            }
        }
        public void Get29_RundownAngleMin(byte[] message)
        {
            string seq = encoding.GetString(message, 217, 2);
            string data = encoding.GetString(message, 219, 5);
            if (seq == "29")
            {
                _29_RundownAngleMin = data;
            }
        }
        public void Get30_RundownAngleMax(byte[] message)
        {
            string seq = encoding.GetString(message, 224, 2);
            string data = encoding.GetString(message, 226, 5);
            if (seq == "30")
            {
                _30_RundownAngleMax = data;
            }
        }
        public void Get31_RundownAngle(byte[] message)
        {
            string seq = encoding.GetString(message, 231, 2);
            string data = encoding.GetString(message, 233, 5);
            if (seq == "31")
            {
                _31_RundownAngle = data;
            }
        }

        public void Get32_CurrentMonitoringMin(byte[] message)
        {
            string seq = encoding.GetString(message, 238, 2);
            string data = encoding.GetString(message, 240, 3);
            if (seq == "32")
            {
                _32_CurrentMonitoringMin = data;
            }
        }
        public void Get33_CurrentMonitoringMax(byte[] message)
        {
            string seq = encoding.GetString(message, 243, 2);
            string data = encoding.GetString(message, 245, 3);
            if (seq == "33")
            {
                _33_CurrentMonitoringMax = data;
            }
        }
        public void Get34_CurrentMonitoringValue(byte[] message)
        {
            string seq = encoding.GetString(message, 248, 2);
            string data = encoding.GetString(message, 250, 3);
            if (seq == "34")
            {
                _34_CurrentMonitoringValue = data;
            }
        }
        public void Get35_SelftapMin(byte[] message)
        {
            string seq = encoding.GetString(message, 253, 2);
            string data = encoding.GetString(message, 255, 6);
            if (seq == "35")
            {
                _35_SelftapMin = data;
            }
        }
        public void Get36_SelftapMax(byte[] message)
        {
            string seq = encoding.GetString(message, 261, 2);
            string data = encoding.GetString(message, 263, 6);
            if (seq == "36")
            {
                _36_SelftapMax = data;
            }
        }
        public void Get37_SelftapTorque(byte[] message)
        {
            string seq = encoding.GetString(message, 269, 2);
            string data = encoding.GetString(message, 271, 6);
            if (seq == "37")
            {
                _37_SelftapTorque = data;
            }
        }
        public void Get38_PrevailTorqueMonitoringMin(byte[] message)
        {
            string seq = encoding.GetString(message, 277, 2);
            string data = encoding.GetString(message, 279, 6);
            if (seq == "38")
            {
                _38_PrevailTorqueMonitoringMin = data;
            }
        }
        public void Get39_PrevailTorqueMonitoringMax(byte[] message)
        {
            string seq = encoding.GetString(message, 285, 2);
            string data = encoding.GetString(message, 287, 6);
            if (seq == "39")
            {
                _39_PrevailTorqueMonitoringMax = data;
            }
        }
        public void Get40_PrevailTorque(byte[] message)
        {
            string seq = encoding.GetString(message, 293, 2);
            string data = encoding.GetString(message, 295, 6);
            if (seq == "40")
            {
                _40_PrevailTorque = data;
            }
        }
        public void Get41_TighteningID(byte[] message)
        {
            string seq = encoding.GetString(message, 301, 2);
            string data = encoding.GetString(message, 303, 10);
            if (seq == "41")
            {
                _41_TighteningID = data;
            }
        }
        public void Get42_JobSequenceNumber(byte[] message)
        {
            string seq = encoding.GetString(message, 313, 2);
            string data = encoding.GetString(message, 315, 5);
            if (seq == "42")
            {
                _42_JobSequenceNumber = data;
            }
        }
        public void Get43_SyncTighteningID(byte[] message)
        {
            string seq = encoding.GetString(message, 320, 2);
            string data = encoding.GetString(message, 322, 5);
            if (seq == "43")
            {
                _43_SyncTighteningID = data;
            }
        }
        public void Get44_SyncTighteningID(byte[] message)
        {
            string seq = encoding.GetString(message, 327, 2);
            string data = encoding.GetString(message, 329, 14);
            if (seq == "44")
            {
                _44_SyncTighteningID = data;
            }
        }
        public void Get45_TimeStamp(byte[] message)
        {
            string seq = encoding.GetString(message, 343, 2);
            string data = encoding.GetString(message, 345, 19);
            if (seq == "45")
            {
                _45_TimeStamp = data;
            }
        }
        public void Get46_DatetimeOfLastChangeInParameterSetSettings(byte[] message)
        {
            string seq = encoding.GetString(message, 364, 2);
            string data = encoding.GetString(message, 366, 19);
            if (seq == "46")
            {
                _46_DatetimeOfLastChangeInParameterSetSettings = data;
            }
        }
        public void Get47_ParameterSetName(byte[] message)
        {
            string seq = encoding.GetString(message, 385, 2);
            string data = encoding.GetString(message, 387, 25);
            if (seq == "47")
            {
                _47_ParameterSetName = data;
            }
        }
        public void Get48_TorqueValuesUnit(byte[] message)
        {
            string seq = encoding.GetString(message, 412, 2);
            string data = encoding.GetString(message, 414, 1);
            if (seq == "48")
            {
                _48_TorqueValuesUnit = data;
            }
        }
        public void Get49_ResultType(byte[] message)
        {
            string seq = encoding.GetString(message, 415, 2);
            string data = encoding.GetString(message, 417, 2);
            if (seq == "49")
            {
                _49_ResultType = data;
            }
        }

        public void Get50_IdentifierResultPart2(byte[] message)
        {
            string seq = encoding.GetString(message, 419, 2);
            string data = encoding.GetString(message, 421, 25);
            if (seq == "50")
            {
                _50_IdentifierResultPart2 = data;
            }
        }
        public void Get51_IdentifierResultPart3(byte[] message)
        {
            string seq = encoding.GetString(message, 446, 2);
            string data = encoding.GetString(message, 448, 25);
            if (seq == "51")
            {
                _51_IdentifierResultPart3 = data;
            }
        }
        public void Get52_IdentifierResultPart4(byte[] message)
        {
            string seq = encoding.GetString(message, 473, 2);
            string data = encoding.GetString(message, 475, 25);
            if (seq == "52")
            {
                _52_IdentifierResultPart4 = data;
            }
        }
        public void Get53_CustomerTighteningErrorCode(byte[] message)
        {
            string seq = encoding.GetString(message, 500, 2);
            string data = encoding.GetString(message, 502, 4);
            if (seq == "53")
            {
                _53_CustomerTighteningErrorCode = data;
            }
        }
        public void Get54_PrevailTorqueCompensateValue(byte[] message)
        {
            string seq = encoding.GetString(message, 506, 2);
            string data = encoding.GetString(message, 508, 6);
            if (seq == "54")
            {
                _54_PrevailTorqueCompensateValue = data;
            }
        }
        public void Get55_TighteningErrorStatus2(byte[] message)
        {
            string seq = encoding.GetString(message, 514, 2);
            string data = encoding.GetString(message, 516, 10);
            if (seq == "55")
            {
                _55_TighteningErrorStatus2 = data;
            }
        }
        public void Get56_CompensatedAngle(byte[] message)
        {
            string seq = encoding.GetString(message, 526, 2);
            string data = encoding.GetString(message, 528, 7);
            if (seq == "56")
            {
                _56_CompensatedAngle = data;
            }
        }
        public void Get57_FinalAngleDecimal(byte[] message)
        {
            string seq = encoding.GetString(message, 535, 2);
            string data = encoding.GetString(message, 537, 7);
            if (seq == "57")
            {
                _57_FinalAngleDecimal = data;
            }
        }
    }

    public class MID0061_LastTighteningResultData
    {
        public MID0061_LastTighteningResultData(byte[] message)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            header = new OpenProtocalHeader(message);
            dataField = new DataField_MID0061(message);
        }
        public OpenProtocalHeader header { get; set; }
        public DataField_MID0061 dataField { get; set; }

    }
}
