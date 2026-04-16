using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RogerTech.Common;
using RogerTech.Tool;

namespace RogerTech.BussnessCore.Bussness
{
    public class PlcProcessReleaseSfcByShopOrder : PlcInProgressBase
    {
        private readonly MesInterface _mesInterface;
        public PlcProcessReleaseSfcByShopOrder(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
        {
            _mesInterface= mesInterface;
        }
        public override void Execute(Group group)
        {
            base.Execute(group);
            Task.Run(() => { DbContext.Info("", $"收到plc请求值StartSignal：[0->1]", 0, PlcGroup.GroupName); });
            WriteFinishSignal(true);
            StringBuilder message = new StringBuilder();
            string sfc = string.Empty;
            var db = DbContext.GetInstance();
            BussnessUtility business = BussnessUtility.GetInstance();
            int resultCode = 30001;
            try
            {
                #region Tag获取和数据校验

                //空循环模式
                if (business.bMesSimulation)
                {
                    resultCode = 0;
                    return;
                }

                List<object> inputs = new List<object>();

                List<object> output = business.MesInvoke(inputs, _mesInterface);

                if ((int)output[0] == 0)
                {
                    resultCode = (int)output[0];
                    if (output[2] is IEnumerable)
                    {
                        foreach (var item in output[2] as IEnumerable)
                        {
                            WriteResult(item.ToString());
                            message.Append($"调用mes接口[ReleaseSfcByShopOrder]释放条码[{item}]成功");
                        }
                    }
                }
                else
                {
                    resultCode = (int)output[0];
                    message.Append($"调用mes接口[ReleaseSfcByShopOrder]释放条码失败！MES代码：[{output[0]}] MES信息[{output[1]}]");
                }
                #endregion
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
            }
            finally
            {
                WriteResult(resultCode);
                WriteFinishSignal(false);
                Task.Run(() => { DbContext.Info(sfc, message.ToString(), resultCode, PlcGroup.GroupName); });
            }
        }
    }
}
