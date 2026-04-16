using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.AuthService.ModelInToBox
{
	[SugarTable("Ctps_Ctrl_Count")]
	public class CtrlCount
	{
		public enum CtrlId
		{
			Pack卡槽 = 1,
			涂胶超时 = 2,
		}
		/// <summary>
		/// 主键ID
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public CtrlId Id { get; set; }


		public int Value { get; set; }


		public string Note { get; set; } = "";

	}
}
