using MiCheckBOMInventory.MiCheckBOMInventory;
using RogerTech.Common;
using RogerTech.Tool;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.BussnessCore.Bussness
{
	public class PlcProcessMiCheckBOMInventory : PlcInProgressBase
	{
		MesInterface MesInterface;
		protected string StationName = ConfigurationManager.AppSettings["StationName"];
		public PlcProcessMiCheckBOMInventory(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
		{
			this.MesInterface = mesInterface;
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
			#region   获取模组码
			Tag sfctag = group.GetTag("SFC");
			if (sfctag == null) OnTagNullError("SFC", group.GroupName);
			sfc = sfctag.Result.Value.ToString();
			if (string.IsNullOrEmpty(sfc))
			{
				message.Append($"传输的模组码为空");
				WriteResult(resultCode);
				return;
			}
			#endregion

			// 根据Config判断是否上传MES
			Tag IsUpload = group.GetTag("IsUpload");
            if (IsUpload == null) OnTagNullError("IsUpload", group.GroupName);
			bool UploadMes = IsUpload.ObjToBool();

			if (UploadMes)
			{
				BussnessUtility bussness = BussnessUtility.GetInstance();
				string productId = " ";
				List<string> cellsns = new List<string>();
				try
				{
					#region Tag获取和数据校验
					List<object> inputs = new List<object>();
					inputs.Add(sfc);
					var inventoryDataArray = new checkBomInventoryData { component = sfc, inventory = sfc, qty = "1" };
					inputs.Add(inventoryDataArray);

                    //空循环模式
                    if (business.bMesSimulation)
                    {
                        resultCode = 0;
                        return;
                    }

                    List<object> output = bussness.MesInvoke(inputs, MesInterface.CheckBOMInventory);
					resultCode = (int)output[0];
					if ((int)(output[0]) == 0)
					{
						resultCode = (int)output[0];
						WriteResult(resultCode);
						message.Append("调用mes接口[CheckBOMInventory]模组校验成功");
						Task.Run(() => { DbContext.Info("", $" {message}", resultCode, PlcGroup.GroupName); });
					}
					else
					{
						resultCode = (int)output[0];
						WriteResult(resultCode);
						message.Append($"调用mes接口[CheckBOMInventory]模组校验失败MES代码[{output[0]}] MES信息[{output[1]}]");
						Task.Run(() => { DbContext.Info("", $" {message}", resultCode, PlcGroup.GroupName); });
						return;
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
					Task.Run(() => { DbContext.Info(productId, message.ToString(), resultCode, PlcGroup.GroupName); });
				}
			}

			WriteFinishSignal(false);
		}
	}
}
