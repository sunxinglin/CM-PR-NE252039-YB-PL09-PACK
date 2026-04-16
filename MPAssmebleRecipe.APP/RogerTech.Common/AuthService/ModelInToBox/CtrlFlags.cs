using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.AuthService.ModelInToBox
{
	/// <summary>
	/// 运行控制标志
	/// </summary>
	[SugarTable("Ctps_Ctrl_Flags")]
	public class CtrlFlags
	{
		public enum FlagId
		{
			Flag_Module_进站_OffOn = 1,
			Flag_Pack_进站_OffOn = 2,
			Flag_Module_校验_OffOn = 3,
			Flag_Pack_校验_OffOn = 4,
			Flag_Pack_涂胶时间_OffOn = 5,
			Flag_Catl_Pack_装备_OffOn = 6,
			Flag_Catl_Pack_收数_OffOn = 7,

		}

		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }

		public bool Value { get; set; }
		public string Note { get; set; } = "";
	}
}
