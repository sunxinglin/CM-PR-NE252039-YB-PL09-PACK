using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.BaseData;

namespace Yee.Services.BaseData
{
    public class DictionaryService
    {
        public readonly AsZeroDbContext _dBContext;

        public DictionaryService(AsZeroDbContext dBContext)
        {
            _dBContext = dBContext;
        }
        /// <summary>
        /// 根据关键字查询所有纪录
        /// </summary>
        /// <returns></returns>
        public async Task<List<Dictionary>> GetAll(string?  key)
        {
            if (key == null)
            {
                var list = await _dBContext.Dictionaries.Where(d => d.IsDeleted == false).ToListAsync();
                return list;
            }
            else
            {
                var list = await _dBContext.Dictionaries.Where(d => d.IsDeleted == false && (d.Code == key || d.Name == key)).ToListAsync();
                return list;
            }
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Dictionary?> GetById(int id)
        {
            var entity =await _dBContext.Dictionaries.FirstOrDefaultAsync(a => a.Id == id);
            return entity;
        }


        public async Task<Dictionary> Add(Dictionary entity)
        {
            var res = await _dBContext.AddAsync(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Dictionary entity)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<Dictionary> Update(Dictionary entity)
        {
            var res = _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<Dictionary> GetByCode(string code)
        {
            var entity = await _dBContext.Dictionaries.FirstOrDefaultAsync(a => a.Code == code);
            return entity;
        }
    }
}
