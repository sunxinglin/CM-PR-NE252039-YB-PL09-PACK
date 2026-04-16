using RogerTech.Common.AuthService.ModelInToBox;
using RogerTech.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RogerTech.Common.AuthService.ModelInToBox.CtrlCount;
using static RogerTech.Common.AuthService.ModelInToBox.CtrlFlags;
using RogerTech.Tool;

namespace RogerTech.BussnessCore.Bussness
{
	public class PlcProcessMiCheckSfcStatusEx : PlcInProgressBase
	{
		MesInterface MesInterface;
		protected string StationName = ConfigurationManager.AppSettings["StationName"];
		public PlcProcessMiCheckSfcStatusEx(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
		{
			this.MesInterface = mesInterface;
		}

		public override void Execute(Group group)
		{

			base.Execute(group);
			Task.Run(() => { DbContext.Info("", $"收到plc请求值StartSignal：[0->1]", 0, PlcGroup.GroupName); });
			StringBuilder message = new StringBuilder();
			string sfc = string.Empty;
			var dbData = DbContext.GetInstance();
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

			#region   获取packCode
			Tag pctag = group.GetTag("PackCode");
			if (pctag == null) OnTagNullError("PackCode", group.GroupName);
			var pc = pctag.Result.Value.ToString();
			if (string.IsNullOrEmpty(pc))
			{
				message.Append($"传输的Pack为空");
				WriteResult(resultCode);
				return;
			}
			#endregion

			#region   获取SoltNo
			Tag soltnotag = group.GetTag("SoltNo");
			if (soltnotag == null) OnTagNullError("SoltNo", group.GroupName);
			var soltno = soltnotag.Result.Value.ToString();
			if (string.IsNullOrEmpty(soltno))
			{
				message.Append($"传输的槽位号为空");
				WriteResult(resultCode);
				return;
			}
			#endregion

			#region 根据卡槽号获取对应的PN
			var pnCode = dbData.Queryable<ModelPnConfig>().First(r => r.SoltNum == int.Parse(soltno)).ModelPn;
			var pn = dbData.Queryable<ModelInfo>().First(r => r.ModuleUid == sfc).ModulePn;
			if (pn != pnCode)
			{
				message.Append($"来料模组PN不一致");
				Task.Run(() => { DbContext.Info("", $" {message}", resultCode, PlcGroup.GroupName); });
				WriteResult(resultCode);
				return;
			}
			#endregion

			#region  根据PackCode获取对应的涂胶时间
			var pcCode = dbData.Queryable<PackInfo>().First(r => r.PackUid == pc);
			if (pcCode == null)
			{
				message.Append($"没有相关的pack");
				Task.Run(() => { DbContext.Info("", $" {message}", resultCode, PlcGroup.GroupName); });
				WriteResult(resultCode);
				return;
			}
			var dt = DateTimeOffset.UtcNow;

			var time = dbData.Queryable<CtrlCount>().First(r => r.Id == CtrlId.涂胶超时);
			if (DateTimeOffset.Compare(dt, pcCode.StartGluingAt.ToUniversalTime()) > time.Value)
			{
				message.Append($"涂胶超时");
				Task.Run(() => { DbContext.Info("", $" {message}", resultCode, PlcGroup.GroupName); });
				WriteResult(99999);
				return;
			}

			var flag = dbData.Queryable<CtrlFlags>().First(it => it.Id == (int)FlagId.Flag_Catl_Pack_装备_OffOn);
			if (flag.Value)
			{

				BussnessUtility bussness = BussnessUtility.GetInstance();
				string productId = " ";
				List<string> cellsns = new List<string>();

				try
				{
					#region Tag获取和数据校验
					Tag sfcT = group.GetTag("SFC");
					if (sfcT == null) OnTagNullError("SFC", group.GroupName);
					sfc = sfcT.Result.Value.ToString();
					if (sfc == null)
					{
						message.Append($"绑定出站失败:传输的模组码为空");
						Task.Run(() => { DbContext.Info("", $" {message}", resultCode, PlcGroup.GroupName); });
						WriteResult(resultCode);
						return;
					}
					List<object> inputs = new List<object>();
					//不从PLC获取的数据，从上位机计算后写入DB
					//Tag test1 = group.GetTag("test1");
					//test1.WriteValue("从数据库获取值、从PLC获取后进行计算");
					inputs = new List<object>();
					inputs.Add(sfc);
					List<object> output = bussness.MesInvoke(inputs, MesInterface.CheckSfcStatus);
					if ((int)output.First() == 0)
					{

						WriteResult(resultCode);
						message.Append("调用mes接口[CheckSfcStatus]获取模组的状态成功");
						Task.Run(() => { DbContext.Info("", $" {message}", resultCode, PlcGroup.GroupName); });

					}
					else
					{
						WriteResult(resultCode);
						message.Append("调用mes接口[CheckSfcStatus]获取模组的状态失败");
						Task.Run(() => { DbContext.Info("", $" {output[1]}", resultCode, PlcGroup.GroupName); });
					}
					#endregion
				}
				catch (Exception ex)
				{
					message.Append(ex.Message);
				}
				finally
				{
					Task.Run(() => { DbContext.Info(productId, message.ToString(), resultCode, PlcGroup.GroupName); });
					WriteResult(resultCode);
					WriteFinishSignal(false);

				}

			}

			#endregion
			WriteFinishSignal(true);
		}
	}
}
