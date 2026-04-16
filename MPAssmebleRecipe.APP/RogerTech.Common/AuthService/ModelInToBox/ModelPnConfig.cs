using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.AuthService.ModelInToBox
{
	[SugarTable("Ctps_Module_Pn_Config")]
	public class ModelPnConfig
	{
		/// <summary>
		/// 主键ID
		/// </summary>
		[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
		public int Id { get; set; }

		public int SoltNum { get; set; }


		[SugarColumn(Length = 50)]
		public string ModelPn { get; set; }
	}
}
