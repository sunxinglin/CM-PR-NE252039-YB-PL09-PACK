using AsZero.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.Common;

namespace Yee.Entitys.Common
{
    public class CommonData : BaseDataModel
    {
        [NotMapped]
        public string? AGVCode { get; set; }
        [NotMapped]
        public virtual string? PackCode { get; set; }

        
    }
}
