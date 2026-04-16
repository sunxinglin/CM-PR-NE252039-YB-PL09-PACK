using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.AuthService.ModelInToBox
{
	[SugarTable("Ctps_Module_Info")]
	public class ModelInfo
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }




		/// <summary>
		///Pack唯一标识
		/// </summary>
		[SugarColumn(Length = 50, IsNullable = true)]
		public string PackUid { get; set; }

		/// <summary>
		///Module唯一标识
		/// </summary>
		[SugarColumn(Length = 50)]
		public string ModuleUid { get; set; }


		/// <summary>
		/// Module料号
		/// </summary>      
		[SugarColumn(Length = 50, IsNullable = true)]
		public string ModulePn { get; set; }


		public int PackSolt { get; set; }

		public bool IsChecked { get; set; }

		public DateTimeOffset CheckAtTime { get; set; }

		public bool IsAssemble { get; set; }

		public DateTimeOffset AssembleAtTime { get; set; }

	}
}
