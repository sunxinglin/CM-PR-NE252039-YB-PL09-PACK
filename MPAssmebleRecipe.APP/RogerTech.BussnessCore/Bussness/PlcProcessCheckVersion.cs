using RogerTech.Common;
using RogerTech.Common.AuthService.ModelInToBox;
using RogerTech.Tool;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RogerTech.Common.AuthService.ModelInToBox.CtrlCount;
using Tag = RogerTech.Tool.Tag;
namespace RogerTech.BussnessCore.Bussness
{
	public class PlcProcessCheckVersion : PlcInProgressBase
	{
		MesInterface MesInterface;
		protected string StationName = ConfigurationManager.AppSettings["StationName"];
		public PlcProcessCheckVersion(string groupName, MesInterface mesInterface) : base(groupName, mesInterface)
		{
			this.MesInterface = mesInterface;
		}

		public override void Execute(Group group)
		{

			base.Execute(group);
			Task.Run(() => { DbContext.Info("", $"收到plc请求值StartSignal：[0->1]", 0, PlcGroup.GroupName); });
			WriteFinishSignal(true);
			StringBuilder message = new StringBuilder();
			string version = string.Empty;
			//  var dbData = DbContext.GetInstance();
			BussnessUtility bussness = BussnessUtility.GetInstance();
			string productId = " ";
			List<string> cellsns = new List<string>();
			int resultCode = 30001;
			try
			{
				#region Tag获取和数据校验
				Tag versionT = group.GetTag("蓝本号");
				if (versionT == null) OnTagNullError("蓝本号", group.GroupName);
				version = versionT.Result.Value.ToString();
				if (version == null)
				{
					message.Append($"传输的蓝本号为空");
					WriteResult(resultCode);
					return;
				}

				Tag PackSolotT = group.GetTag("PackSolt");
				if (PackSolotT == null) OnTagNullError("PackSolt", group.GroupName);
				var packSolt = versionT.Result.Value.ToString();
				if (packSolt == null)
				{
					message.Append($"传输的packSolt为空");
					WriteResult(resultCode);
					return;
				}
				var ss = DbContext.GetInstance().Queryable<CtrlCount>().Where(r => r.Id == CtrlId.Pack卡槽).First();
				var mespackSolt = ss.Value;

				Tag versionTag = group.GetTag("蓝本号");
				if (versionTag == null) OnTagNullError("蓝本号", group.GroupName);
				versionTag.WriteValue(mespackSolt);


				Tag packSoltTag = group.GetTag("PackSolt");
				if (packSoltTag == null) OnTagNullError("PackSolt", group.GroupName);
				versionTag.WriteValue(CtrlId.Pack卡槽);


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
	}
}
