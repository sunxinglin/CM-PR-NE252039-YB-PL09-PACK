using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Yee.Entitys.BaseData;

namespace Yee.Services.BaseData
{
    public class CategoryTypeService
    {
        public readonly AsZeroDbContext _dBContext;
        public CategoryTypeService(AsZeroDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public CategoryType? GetById(int id)
        {
            var entity = _dBContext.CategoryTypes.FirstOrDefault(a => a.Id == id);
            return entity;
        }

        public async Task<CategoryType> Add(CategoryType entity)
        {
            var res = await _dBContext.AddAsync(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(CategoryType entity)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<CategoryType> Update(CategoryType entity)
        {
            var res = _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }
        /// <summary>
        /// 加载全部类型列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<CategoryType>> LordList()
        {
           return await _dBContext.CategoryTypes.Where(o=>!o.IsDeleted).ToListAsync();
        }
    }
}
