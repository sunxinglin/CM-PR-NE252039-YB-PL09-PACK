using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;
using DocumentFormat.OpenXml.Validation;
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
    public class ProductService
    {
        public readonly AsZeroDbContext _dBContext;
        private readonly SysLogService sys_LogService;

        public ProductService(AsZeroDbContext dBContext, SysLogService sys_LogService)
        {
            _dBContext = dBContext;
            this.sys_LogService = sys_LogService;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_Product>> GetAll(string? key)
        {
            if (key == null)
            {
                var list = await _dBContext.Products.Where(d => d.IsDeleted == false).Include(d => d.Type).ToListAsync();
                return list;
            }
            else
            {
                var list = await _dBContext.Products.Where(d => d.IsDeleted == false && (d.Code.Contains(key) || d.Name.Contains(key))).Include(d => d.Type).ToListAsync();
                return list;
            }
        }

        /// <summary>
        /// 查找没有配置工艺的产品列表
        /// </summary>
        public async Task<List<Base_Product>> LoadListForNoFlow()
        {
            var flowProductIdList = _dBContext.Base_Flows.Where(d => d.IsDeleted == false && d.ProductId > 0).Select(f => f.ProductId).ToList();
            var list = await _dBContext.Products.Where(d => d.IsDeleted == false && !(flowProductIdList.Contains(d.Id))).ToListAsync();
            return list;
        }
        /// <summary>
        /// 根据packcode获取第一个匹配规则的产品
        /// </summary>
        public async Task<Base_Product?> GetByPackcode(string prductcode)
        {
            var productlist = await _dBContext.Products.Where(o => !o.IsDeleted).ToListAsync();

            foreach (var item in productlist)
            {
                if (item.PackPNRule?.Length == prductcode.Length && prductcode.StartsWith(item.PackPNRule.TrimEnd('*')))
                {
                    return item;
                }

            }
            return null;
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        public async Task<Base_Product?> GetById(int id)
        {
            var entity = await _dBContext.Products.FindAsync(id);
            return entity;
        }

        /// <summary>
        /// 通过Code查询
        /// </summary>
        public async Task<Base_Product?> GetByCode(string code)
        {
            var entity = await _dBContext.Products.FirstOrDefaultAsync(p => !p.IsDeleted && p.Code == code);
            return entity;
        }

        public async Task<Base_Product> Add(Base_Product entity, string op)
        {
            var res = await _dBContext.AddAsync(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增产品, Message = $"新增产品:{entity.Name}", Operator = op });
            await _dBContext.SaveChangesAsync();

            return res.Entity;
        }

        public async Task Delete(Base_Product entity, string op)
        {
            entity.IsDeleted = true;
            entity.Type = null;

            _dBContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除产品, Message = $"删除产品:{entity.Name}", Operator = op });
            await _dBContext.SaveChangesAsync();
        }

        public async Task<Base_Product> Update(Base_Product entity, string op)
        {
            entity.Type = null;

            var res = _dBContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改产品, Message = $"修改产品:{entity.Name}", Operator = op });
            await _dBContext.SaveChangesAsync();
            return res.Entity;
        }

        public async Task<Response<Base_Product>> GetProductByPackRule(string Code)
        {
            Response<Base_Product> result = new Response<Base_Product>();
            var entities = _dBContext.Products.Where(p => !p.IsDeleted).ToList();
            var entity = entities.FirstOrDefault(s => (s.PackPNRule.Length == Code.Length || s.PackOutCodeRule.Length == Code.Length)
                            && (Code.StartsWith(s.PackPNRule.Trim('*')) || Code.StartsWith(s.PackOutCodeRule.Trim('*'))));

            if (entity != null)
            {
                result.Code = 200;
                result.Result = entity;
            }
            else
            {
                result.Code = 500;
                result.Message = "未找到当前输入条码对应的条码规则";
            }

            return result;
        }
    }
}
