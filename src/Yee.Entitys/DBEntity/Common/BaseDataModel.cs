using AsZero.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yee.Entitys.DBEntity.Common
{
    public class BaseDataModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string? Remark { get; set; }
        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public int? CreateUserID { get; set; }
        public User? CreateUser { get; set; }
        public DateTime? UpdateTime { get; set; } = DateTime.Now;
        public int? UpdateUserID { get; set; }
        public User? UpdateUser { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeleteTime { get; set; }
        public int? DeleteUserID { get; set; }
        public User? DeleteUser { get; set; }
    }
}
