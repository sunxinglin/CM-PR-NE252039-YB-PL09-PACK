using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DTOS;
using Yee.Entitys.Production;
using Yee.Services.Request;

namespace Yee.Services.Production
{
    public class ProResourceService
    {
        public readonly AsZeroDbContext _dBContext;
        public ProResourceService(AsZeroDbContext dBContext)
        {
            _dBContext = dBContext;
        }

        public async Task<IList<Base_ProResource>> GetList()
        {
            var list = await this._dBContext.Base_ProResources
                .Where(r => !r.IsDeleted)
                .ToListAsync();
            return list;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_ProResource>> GetAll(string? key)
        {
            if (key == null)
            {
                var list = await _dBContext.Base_ProResources.Where(d => d.IsDeleted == false).OrderBy(o=>o.Name).ToListAsync();
                return list;
            }
            else
            {
                var list = await _dBContext.Base_ProResources.Where(d => d.IsDeleted == false && (d.Code.Contains(key) || d.Name.Contains(key))).ToListAsync();
                return list;
            }
        }
        /// 通过ID查询
        /// </summary>
        public async Task<Base_ProResource?> GetById(int id)
        {
            var entity = await _dBContext.Base_ProResources.FindAsync(id);
            return entity;
        }


        public async Task<Base_ProResource> Add(Base_ProResource entity)
        {
            var res = await _dBContext.AddAsync(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task Delete(Base_ProResource entity)
        {
            entity.IsDeleted = true;
            _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<Base_ProResource> Update(Base_ProResource entity)
        {
            var res = _dBContext.Update(entity);
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<List<Base_ProResource>> GetProResourceByStation(string stationCode, ProResourceTypeEnum proResourceType)
        {
            var List = _dBContext.Base_ProResources.Where(s => s.StationCode == stationCode && s.IsDeleted == false
            && s.ProResourceType == proResourceType && s.IsEnable).ToList();
            return List;
        }

        /// <summary>
        /// 通过stationid 来获取
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public async Task<List<Base_ProResource>> GetProResourceByStation(string stationCode)
        {
            var List = _dBContext.Base_ProResources.Where(s => s.StationCode == stationCode && s.IsDeleted == false).ToList();
            return List;
        }

        public async Task<Base_ProResource> GetGunByDeviceNo(string deviceno)
        {
            return  _dBContext.Base_ProResources.FirstOrDefault(o=>!o.IsDeleted && o.ProResourceType == ProResourceTypeEnum.拧紧枪 && o.DeviceNo==deviceno);
        }


        /// <summary>
        /// 通过stationid 和type 来获取
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns></returns>
        public async Task<List<Base_ProResource>> GetProResourceByStepIdAndType(int stepID, ProResourceTypeEnum ProResourceType)
        {
            var stations = _dBContext.Base_Stations.Where(s => s.StepId == stepID && !s.IsDeleted).Select(s=>s.Code).ToList();
           var all= _dBContext.Base_ProResources.Where(s => !s.IsDeleted).ToList();
            var List = _dBContext.Base_ProResources.Where(s => stations.Contains(s.StationCode) && s.IsDeleted == false && s.ProResourceType == (ProResourceTypeEnum)ProResourceType).ToList();
            return List;
        }
    }
}
