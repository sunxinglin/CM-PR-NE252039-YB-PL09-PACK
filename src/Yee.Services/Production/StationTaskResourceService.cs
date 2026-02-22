using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class StationTaskResourceService
    {
        public readonly AsZeroDbContext _dBContext;
        public StationTaskResourceService(AsZeroDbContext dBContext)
        {
            this._dBContext = dBContext;
        }


        //public async Task<Base_StationTaskResource> Add(Base_StationTaskResource entity)
        //{
        //    var res = await _dBContext.AddAsync(entity);
        //    await _dBContext.SaveChangesAsync();
        //    return res.Entity;
        //}

        //public async Task Delete(Base_StationTaskResource entity)
        //{
        //    entity.IsDeleted = true;
        //    _dBContext.Update(entity);
        //    await _dBContext.SaveChangesAsync();
        //}

        //public async Task<Base_StationTaskResource> Update(Base_StationTaskResource entity)
        //{
        //    var res = _dBContext.Update(entity);
        //    await _dBContext.SaveChangesAsync();
        //    return res.Entity;
        //}

        ///// <summary>
        ///// 通过ID查询
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //public async Task<Base_StationTaskResource> GetById(int id)
        //{
        //    var entity = await _dBContext.Base_StationTaskResources.FindAsync(id);

        //    return entity;
        //}

        //public async  Task<Base_StationTaskResource> GetByStationTaskId(int taskId)
        //{
        //    var entity = await _dBContext.Base_StationTaskResources.Where(d => !d.IsDeleted && d.Base_StationTaskId == taskId).FirstOrDefaultAsync();
        //    return entity;
        //}
    }
}
