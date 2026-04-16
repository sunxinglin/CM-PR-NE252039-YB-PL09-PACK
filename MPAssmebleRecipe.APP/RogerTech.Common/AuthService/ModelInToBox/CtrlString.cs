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
	[SugarTable("Ctps_Ctrl_String")]
	public class CtrlString
	{

		public enum StringId
		{

			上传代码_Code1 = 1,
			上传代码_Code2 = 2,
			上传代码_Code3 = 3,
			上传代码_Code4 = 4,
			上传代码_Code5 = 5,
			上传代码_Code6 = 6,
			上传代码_Code7 = 7,
			上传代码_Code8 = 8,
			上传代码_Code9 = 9,
			上传代码_Code10 = 10,


		}

		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }

		public string Value { get; set; }
		public string Note { get; set; } = "";
	}
}
