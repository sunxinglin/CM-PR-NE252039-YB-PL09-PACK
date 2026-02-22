using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Entitys.Production;
using Yee.Services.Request;

namespace Yee.Services.Production
{
    public class PackService
    {
        public readonly AsZeroDbContext _dBContext;
        public PackService(AsZeroDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        /// <summary>
        /// 通过关键字查询所有
        /// </summary>
        public async Task<List<Base_Pack>> GetAll(string? key)
        {
            if (key == null)
            {
                var list = await _dBContext.Base_Packs.Where(d => d.IsDeleted == false).ToListAsync();
                return list;
            }
            else
            {
                var list = await _dBContext.Base_Packs.Where(d => d.IsDeleted == false && (d.Code.Contains( key) || d.Name.Contains(key))).ToListAsync();
                return list;
            }
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_Pack?> GetById(int id)
        {
            var entity =await _dBContext.Base_Packs.FindAsync(id);
            return entity;
        }

        public async Task<Base_Pack?>Add(Base_Pack entity)
        {
            var res = await _dBContext.AddAsync(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_Pack entity)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<Base_Pack?> Update(Base_Pack entity)
        {
            var res = _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }
    }
}
