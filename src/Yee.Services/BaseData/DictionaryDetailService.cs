using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.BaseData;
using Yee.Services.Request;

namespace Yee.Services.BaseData
{
    public class DictionaryDetailService
    {
        public readonly AsZeroDbContext _dBContext;
        public DictionaryDetailService(AsZeroDbContext dBContext)
        {
            _dBContext = dBContext;
        }
        /// <summary>
        /// 根据关键字查询所有纪录
        /// </summary>
        /// <returns></returns>
        public async Task<List<DictionaryDetail>?> GetAll(string? key)
        {
            if (key == null)
            {
                var list = await _dBContext.DictionaryDetails.Where(d => d.IsDeleted == false).ToListAsync();
                return list;
            }
            else
            {
                var list = await _dBContext.DictionaryDetails.Where(d => d.IsDeleted == false && (d.Code == key || d.Name == key)).ToListAsync();
                return list;
            }
        }
        /// <summary>
        /// 外键查询
        /// </summary>
        public async Task<List<DictionaryDetail>>GetByFK(int FK_Id)
        {
            var entity=await _dBContext.DictionaryDetails.Where(d=>d.IsDeleted==false&&d.DictionaryId==FK_Id).ToListAsync();
            return entity;
        }
        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<DictionaryDetail?> GetById(int id)
        {
            var entity =await _dBContext.DictionaryDetails.FindAsync(id);
            return entity;
        }

        public async Task<List<DictionaryDetail>> GetAllByDictionaryId(int dictionaryId)
        {
            var list = await _dBContext.DictionaryDetails.Where(d=>d.DictionaryId == dictionaryId && d.IsDeleted == false).ToListAsync();
            return list;
        }

        public async Task<List<DictionaryDetail>> GetListByType(GetListByTypeInput input)
        {
            var query = _dBContext.DictionaryDetails.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(input.TypeCode))
            {
                query = query.Where(a => a.Dictionary.Code==input.TypeCode);
            }
            
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<DictionaryDetail> Add(DictionaryDetail entity)
        {
            var res = await _dBContext.AddAsync(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(DictionaryDetail entity)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<DictionaryDetail> Update(DictionaryDetail entity)
        {
            var res = _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }
    }
}
