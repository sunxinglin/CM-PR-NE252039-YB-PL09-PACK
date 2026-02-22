//using AsZero.DbContexts;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Yee.Entitys.BaseData;
//using Yee.Services.Request;

//namespace Yee.Services.BaseData
//{
//    public class UnitService
//    {
//        public readonly AsZeroDbContext _dBContext;
//        public UnitService(AsZeroDbContext dbContext)
//        {
//            _dBContext = dbContext;
//        }

//        /// <summary>
//        /// 通过关键字查询
//        /// </summary>
//        public async Task<List<Unit>> GetAll(string? key)
//        {
//            if (key == null)
//            {
//                var list = await _dBContext.Units.Where(d => d.IsDeleted == false).ToListAsync();
//                return list;
//            }
//            else
//            {
//                var list = await _dBContext.Units.Where(d => d.IsDeleted == false && (d.Code == key || d.Name == key)).ToListAsync();
//                return list;
//            }
//        }
//        /// <summary>
//        /// 通过ID查询
//        /// </summary>
//        public async Task<Unit?> GetById(int id)
//        {
//            var entity =await _dBContext.Units.FindAsync(id);
//            return entity;
//        }

//        public async Task<Unit> Add(Unit entity)
//        {
//            var res = await _dBContext.AddAsync(entity);
//            await _dBContext.SaveChangesAsync();
//            return res.Entity;
//        }

//        public async Task Delete(Unit entity)
//        {
//            entity.IsDeleted = true;
//            _dBContext.Update(entity);
//            await _dBContext.SaveChangesAsync();
//        }

//        public async Task<Unit> Update(Unit entity)
//        {
//            var res = _dBContext.Update(entity);
//            await _dBContext.SaveChangesAsync();
//            return res.Entity;
//        }

//        public async Task<List<Unit>> getUnitBase()
//        {
//            var res = await _dBContext.Units.Where(u => u.IsBase && !u.IsDeleted).ToListAsync();
//            return res;
//        }

//        public async Task<List<Unit>> GetUnitByType(int typeId)
//        {
//            var list = await _dBContext.Units.Where(u => u.Type == typeId).ToListAsync();
//            return list;
//        }
//    }
//}
