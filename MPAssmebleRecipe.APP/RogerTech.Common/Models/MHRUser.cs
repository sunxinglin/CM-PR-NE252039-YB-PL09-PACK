using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogerTech.Common.Models
{
    public class MHRUser
    {
        /// <summary>
        /// 工号
        /// </summary>
        [SugarColumn(Length = 50)]
        public string work_id { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        [SugarColumn(Length = 50)]
        public string card_id { get; set; }
        /// <summary>
        /// 权限等级
        /// </summary>
        [SugarColumn(Length = 50)]
        public string access_level { get; set; }


        /// <summary>
        /// 下发时间
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public DateTime? updatetime { get; set; }
    }
}
