using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.AuthService.ModelInToBox
{
	[SugarTable("Ctps_Pack_Info")]
	public class PackInfo
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

		public int AGVCode { get; set; }

		public DateTimeOffset StartGluingAt { get; set; }


		public bool IsChcek { get; set; }

		public DateTimeOffset ChceckAt { get; set; }

		public bool IsAssemble { get; set; }

		public DateTimeOffset AssembleAt { get; set; }
		/// <summary>
		///Module唯一标识
		/// </summary>
		[SugarColumn(Length = 10000)]
		public PackUpLoadData UpLoadData { get; set; }


	}
}
