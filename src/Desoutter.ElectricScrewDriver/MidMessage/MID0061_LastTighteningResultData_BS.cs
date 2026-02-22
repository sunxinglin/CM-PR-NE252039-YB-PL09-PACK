using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desoutter.ElectricScrewDriver.MidMessage
{
    public class DataField_MID0061_BS
    {
        public DataField_MID0061_BS(byte[] message)
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
        public string _07_OkCounterLimit { get; set; } = string.Empty;
        public string _08_OkCounterValue { get; set; } = string.Empty;
        public string _09_TightenStatus { get; set; } = string.Empty;
        public string _10_TorqueStatus { get; set; } = string.Empty;

        /// <summary>
        /// 总是否合格
        /// </summary>
        public string _11_AngleStatus { get; set; } = string.Empty;
        public string _12_MinTorque { get; set; } = string.Empty;

        /// <summary>
        /// 扭矩否合格
        /// </summary>
        public string _13_MaxTorque { get; set; } = string.Empty;

        /// <summary>
        /// 角度是否合格
        /// </summary>
        public string _14_TargetTorque { get; set; } = string.Empty;

        /// <summary>
        /// 扭矩工艺下限
        /// </summary>
        public string _15_FinalTorque { get; set; } = string.Empty;//扭矩工艺下限
        /// <summary>
        /// 扭矩工艺上限
        /// </summary>
        public string _16_MinAngle { get; set; } = string.Empty;//扭矩工艺上限

        /// <summary>
        /// 目标扭矩值
        /// </summary>
        public string _17_MaxAngle { get; set; } = string.Empty;//
        /// <summary>
        /// 扭矩结果
        /// </summary>
        public string _18_TargetAngle { get; set; } = string.Empty;//扭矩结果
        /// <summary>
        /// 工艺角度下限
        /// </summary>
        public string _19_FinalAngle { get; set; } = string.Empty;//工艺角度下限
        /// <summary>
        /// 工艺角度上限
        /// </summary>
        public string _20_TightenTimeSpan { get; set; } = string.Empty;//工艺角度上限
        /// <summary>
        /// 目标角度值
        /// </summary>
        public string _21_LastChangeTime { get; set; } = string.Empty;//
        /// <summary>
        /// 角度结果
        /// </summary>
        public string _22_CounterStatus { get; set; } = string.Empty;//角度结果

        public string _23_TightenID { get; set; } = string.Empty;
       

        /// <summary>
        /// 带小数点的角度结果
        /// </summary>
        //public string _57_FinalAngleDecimal { get; set; } = string.Empty;//带小数点的角度结果

        public ASCIIEncoding encoding { get; set; }

        private void Create(byte[] message)
        {
            this._01_CellID = GetString(message, 22, 4);
            this._02_ChannelID = GetString(message, 28, 2);
            this._03_TorqueControllerName = GetString(message, 32, 25);
            this._04_VINNumber = GetString(message, 59, 25);
            this._05_JobID = GetString(message, 86, 2);
            this._06_ParameterSetNumber = GetString(message, 90, 3);
            this._07_OkCounterLimit = GetString(message, 95, 4);
            this._08_OkCounterValue = GetString(message, 101, 4);
            this._09_TightenStatus = GetString(message, 107, 1);
            this._10_TorqueStatus = GetString(message, 110, 1);
            this._11_AngleStatus = GetString(message, 113, 1);
            this._12_MinTorque = GetString(message, 116, 6);
            this._13_MaxTorque = GetString(message, 124, 6);
            this._14_TargetTorque = GetString(message, 132, 6);
            this._15_FinalTorque = GetString(message, 140, 6);
            this._16_MinAngle = GetString(message, 148, 5);
            this._17_MaxAngle = GetString(message,155, 5);
            this._18_TargetAngle = GetString(message, 162, 5);
            this._19_FinalAngle = GetString(message, 169, 5);
            this._20_TightenTimeSpan = GetString(message, 176, 19);
            this._21_LastChangeTime = GetString(message, 197, 19);
            this._22_CounterStatus = GetString(message, 218, 1);
            this._23_TightenID = GetString(message, 221, 10);

        }

        private string GetString(byte[] message, int sourceIndex, int size)
        {
            return encoding.GetString(message, sourceIndex, size);
        }
      
    }

    public class MID0061_LastTighteningResultData_BS
    {
        public MID0061_LastTighteningResultData_BS(byte[] message)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            header = new OpenProtocalHeader(message);
            dataField = new DataField_MID0061_BS(message);
        }
        public OpenProtocalHeader header { get; set; }
        public DataField_MID0061_BS dataField { get; set; }

    }
}
