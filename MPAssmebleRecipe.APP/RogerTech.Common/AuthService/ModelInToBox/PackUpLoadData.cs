using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.AuthService.ModelInToBox
{
	[SugarTable("Ctps_Pack_Pro_Info")]
	public class PackUpLoadData
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }
		/// <summary>
		///Module唯一标识
		/// </summary>
		[SugarColumn(Length = 50)]
		public string PackUid { get; set; }

		/// <summary>
		///Module唯一标识
		/// </summary>
		[SugarColumn(Length = 50)]
		public string ModuleUid { get; set; }


		public float Pressure1 { get; set; }

		public float Pressure2 { get; set; }
		public float Pressure3 { get; set; }
		public float Pressure4 { get; set; }
		public float Pressure5 { get; set; }
		public float Pressure6 { get; set; }
		public float Pressure7 { get; set; }
		public float Pressure8 { get; set; }
		public float Pressure9 { get; set; }
		public float Pressure10 { get; set; }
		public float Length1 { get; set; }
		public float Length2 { get; set; }
		public float Length3 { get; set; }
		public float Length4 { get; set; }
		public float Length5 { get; set; }
		public float Length6 { get; set; }
		public float Length7 { get; set; }
		public float Length8 { get; set; }
		public float Length9 { get; set; }
		public float Length10 { get; set; }

	}
}
