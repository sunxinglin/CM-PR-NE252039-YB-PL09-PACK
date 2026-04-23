using RogerTech.Common;
using RogerTech.Share;
using RogerTech.Tool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.BussnessCore.Bussness
{
    /// <summary>
    /// 进站
    /// </summary>
    public class PlcProcessMiFindCustomAndSfcData : PlcInProgressBase
    {
        private readonly MesInterface _mesInterface;
        public PlcProcessMiFindCustomAndSfcData(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
        {
            _mesInterface = mesInterface;
        }

        public override void Execute(Group group)
        {
            base.Execute(group);
            Task.Run(() => { DbContext.Info("", $"收到plc请求值StartSignal：[0->1]", 0, PlcGroup.GroupName); });
            WriteFinishSignal(true);
            StringBuilder message = new StringBuilder();
            string sfc = string.Empty;
            BussnessUtility business = BussnessUtility.GetInstance();
            int resultCode = 30001;
            try
            {
                #region Tag获取和数据校验
                if (!TryGetRequiredStringTagValue(
                        group,
                        "SFC",
                        message,
                        ref resultCode,
                        30001,
                        30001,
                        "SFC变量读取异常",
                        "绑定进站失败:传输的Pack码为空",
                        out sfc))
                {
                    return;
                }

                //空循环模式
                if (business.bMesSimulation)
                {
                    resultCode = 0;
                    return;
                }

                var db = DbContext.GetInstance();
                List<object> inputs = new List<object> { sfc };
                List<object> output = business.MesInvoke(inputs, _mesInterface);

                // 根据MES返回码决定最终写回PLC的结果码和日志信息
                if ((int)output[0] == 0)
                {
                    resultCode = (int)output[0];
                    message.Append("调用mes接口[MiFindCustomAndSfcData]进站成功");
                }
                else
                {
                    resultCode = (int)output[0];
                    message.Append($"调用mes接口[MiFindCustomAndSfcData]进站失败MES代码[{output[0]}] MES信息[{output[1]}]");
                }
                #endregion
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
            }
            finally
            {
                // 统一回写PLC结果，并复位FinishSignal（表示本次处理结束）
                WriteResult(resultCode);
                WriteFinishSignal(false);
                Task.Run(() => { DbContext.Info(sfc, message.ToString(), resultCode, PlcGroup.GroupName); });
            }
        }
    }
}
