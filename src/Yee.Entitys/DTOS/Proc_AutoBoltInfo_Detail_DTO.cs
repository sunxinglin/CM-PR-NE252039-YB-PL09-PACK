using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Production;

namespace Yee.Entitys.DTOS
{
    public class Proc_AutoBoltInfo_Detail_DTO : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Proc_StationTask_MainId { get; set; }

        /// <summary>
        ///  拧紧结果
        /// </summary>
        public bool ResultIsOK;

        /// <summary>
        ///  程序号
        /// </summary>
        public int ProgramNo;

        /// <summary>
        /// 扭矩值
        /// </summary>
        public decimal FinalTorque;

        /// <summary>
        /// 常数1反馈
        /// </summary>
        public int Constant1;

        /// <summary>
        /// 扭矩结果：0 偏小； 1 正好； 2偏大；
        /// </summary>
        public int TorqueStatus;

        /// <summary>
        /// 角度值
        /// </summary>
        public decimal FinalAngle;

        /// <summary>
        /// 常数1反馈
        /// </summary>
        public int Constant2;

        /// <summary>
        /// 角度结果：0 偏小； 1 正好； 2偏大；
        /// </summary>
        public int AngleStatus;

        /// <summary>
        /// 最小扭矩
        /// </summary>
        public decimal TorqueRate_Min;

        /// <summary>
        /// 目标扭矩
        /// </summary>
        public decimal TargetTorqueRate;

        /// <summary>
        /// 最大扭矩
        /// </summary>
        public decimal TorqueRate_Max;

        /// <summary>
        /// 最小角度
        /// </summary>
        public decimal Angle_Min;

        /// <summary>
        /// 目标角度
        /// </summary>
        public decimal TargetAngle;

        /// <summary>
        /// 最大角度
        /// </summary>
        public decimal Angle_Max;

        public int OrderNo;



        private string _Status = "";
        public string Status
        {
            get => _Status;
            set
            {
                _Status = value;
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }



        public int CreateUserID { get; set; }
        public DateTime CreateTime { get; set; }

        public string PackCode { get; set; }
        public string AGVCode { get; set; }
        public int Base_StationTaskId { get; set; }
        public string TaskName { get; set; }
        public int StationId { get; set; }
        public int CurStepNo { get; set; }
        public int DeviceNo { get; set; }
    }
}
