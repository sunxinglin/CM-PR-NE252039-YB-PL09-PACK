using AsZero.Core.Entities;
using AsZero.Core.Services.Repos;
using AsZero.Core.Services.Sys_Logs;
using AsZero.DbContexts;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DTOS.StationTaskDataDTOS;
using Yee.Entitys.Production;

namespace Yee.Services.Production
{
    public class StationTaskTightenByImageService
    {
        private readonly AsZeroDbContext _dbContext;
        private readonly SysLogService sys_LogService;

        public StationTaskTightenByImageService(AsZeroDbContext dbContext, SysLogService sys_LogService)
        {
            _dbContext = dbContext;
            this.sys_LogService = sys_LogService;
        }

        public async Task<Base_StationTask_TightenByImage> Add(Base_StationTask_TightenByImage entity, string op)
        {
            var res = await _dbContext.AddAsync(entity);

            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.新增配方, Message = $"新增工位图示拧紧任务，任务名称:{entity.TaskName}", Operator = op });
            await _dbContext.SaveChangesAsync();
            return res.Entity;
        }

        /// <summary>
        /// 通过关键字查询
        /// </summary>
        public async Task<List<Base_StationTask_TightenByImage>> GetScrewListByStationTaskID(int stationTaskID)
        {
            var list = await _dbContext.Base_StationTask_TightenByImages.Where(d => d.IsDeleted == false && d.StationTaskId == stationTaskID).ToListAsync();
            return list;
        }


        public async Task Delete(Base_StationTask_TightenByImage entity, string op)
        {
            entity.IsDeleted = true;
            _dbContext.Update(entity);
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.删除配方, Message = $"删除工位图示拧紧任务，任务名称:{entity.TaskName}", Operator = op });
            await _dbContext.SaveChangesAsync();
        }


        public async Task<Base_StationTask_TightenByImage> Update(Base_StationTask_TightenByImage entity, string op)
        {
            var resold = _dbContext.Base_StationTask_TightenByImages.Where(a => !a.IsDeleted && a.Id == entity.Id).AsNoTracking().FirstOrDefault();
            await sys_LogService.AddLog(new SysLog() { LogType = Sys_LogType.修改配方, Message = $"修改前工位图示拧紧任务，任务名称:{resold.TaskName},程序号:{resold.ProgramNo},螺栓总量:{resold.ScrewNum},上传代码{resold.UpMesCode}", Operator = op });
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// 通过ID查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Base_StationTask_TightenByImage> GetById(int id)
        {
            var entity = await _dbContext.Base_StationTask_TightenByImages.FindAsync(id);
            return entity;
        }


        public async Task<Response> UploadImage(UploadImageDTO dto)
        {
            var response = new Response();
            try
            {
               
                if (dto.ImageFile == null )
                {
                    response.Code = 400;
                    response.Message = "未提供图片";
                    return response;
                }

                var image = dto.ImageFile;
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowedTypes.Contains(image.ContentType))
                {
                    response.Code = 400;
                    response.Message = "仅支持 JPG、PNG、GIF 格式";
                    return response;
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                // 生成唯一文件名（避免覆盖）
                var fileName = Path.GetFileName(image.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                // 保存文件到服务器
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // 返回文件访问路径
                var fileUrl = $"/images/{fileName}";
                var task = _dbContext.Base_StationTask_TightenByImages.FirstOrDefault(f => f.StationTaskId == dto.TaskId);
                if (task == null)
                {
                    response.Code = 500;
                    response.Message = "未找到任务";
                    return response;
                }
                task.ImageUrl = fileUrl;
                _dbContext.Update(task);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response<IList<TightenByImageDto>>>  LoadTaskList(string? productPn, string? stepCode)
        {
            var response = new Response<IList<TightenByImageDto>>();

            try
            {
                Base_Product? product = null;
                List < TightenByImageDto> dto = new List<TightenByImageDto>();
                if (!string.IsNullOrEmpty(productPn))
                {
                    product = await _dbContext.Products.FirstOrDefaultAsync(f => !f.IsDeleted && f.Code == productPn);
                }
                if(product == null)
                {
                        dto = !string.IsNullOrEmpty(stepCode)
                              ? await _dbContext.Base_StationTasks.Include(i => i.Step).Where(w => !w.IsDeleted && w.Step != null && w.Step.Code == stepCode && w.Type == StationTaskTypeEnum.图示拧紧).Select(s => new TightenByImageDto { Id = s.Id, TaskName = s.Name ?? "" }).ToListAsync()
                              : await _dbContext.Base_StationTasks.Include(i => i.Step).Where(w => !w.IsDeleted && w.Type == StationTaskTypeEnum.图示拧紧).Select(s => new TightenByImageDto { Id = s.Id, TaskName = s.Name ?? "" }).ToListAsync();
                }
                else
                {
                    dto = !string.IsNullOrEmpty(stepCode)
                             ? await _dbContext.Base_StationTasks.Include(i => i.Step).Where(w => !w.IsDeleted && w.ProductId == product.Id && w.Step != null && w.Step.Code == stepCode && w.Type == StationTaskTypeEnum.图示拧紧).Select(s => new TightenByImageDto { Id = s.Id, TaskName = s.Name ?? "" }).ToListAsync()
                             : await _dbContext.Base_StationTasks.Include(i => i.Step).Where(w => !w.IsDeleted && w.ProductId == product.Id && w.Type == StationTaskTypeEnum.图示拧紧).Select(s => new TightenByImageDto { Id = s.Id, TaskName = s.Name ?? "" }).ToListAsync();

                }
                response.Result = dto;
                return response;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response<LayoutInfoDto>> LoadLayout(int taskId)
        {
            var response = new Response<LayoutInfoDto>();

            try
            {
                var task = await _dbContext.Base_StationTask_TightenByImages.FirstOrDefaultAsync(f => f.StationTaskId == taskId);
                if(task == null)
                {
                    response.Code = 500;
                    response.Message = "图示拧紧任务未配置，请检查配方";
                    return response;
                }
 
                var resp = new LayoutInfoDto
                {
                    TaskId = task.StationTaskId,
                    ImageUrl = task.ImageUrl ?? "",
                    CanvasLayout = task.Layout
                };

                response.Result = resp;

            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        private IFormFile ConvertToIFormFile(string localFilePath)
        {
            IFormFile formFile = null!;
            // 读取本地文件流
            using (var fileStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
            {
                var fileName = Path.GetFileName(localFilePath);
                var contentType = GetContentType(fileName); // 需自定义方法获取MIME类型

                // 创建 FormFile 实例（实现了 IFormFile）
                formFile = new FormFile(
                    baseStream: fileStream,
                    baseStreamOffset: 0,
                    length: fileStream.Length,
                    name: "file", // 对应表单中的键名（与控制器参数名一致）
                    fileName: fileName
                )
                {
                    ContentType = contentType
                };

            }
            // 获取文件名和内容类型

            return formFile;
        }
        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream" // 默认二进制流
            };
        }
        public async Task<Response> SaveLayout(LayoutInfoDto dto)
        {
            var response = new Response();

            try
            {
                var task = await _dbContext.Base_StationTask_TightenByImages.FirstOrDefaultAsync(f => f.StationTaskId == dto.TaskId);
                if (task == null)
                {
                    response.Code = 500;
                    response.Message = "图示拧紧任务未配置，请检查配方";
                    return response;
                }
                task.LayoutJson = JsonConvert.SerializeObject(dto.CanvasLayout);
                _dbContext.Update(task);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response<string>> GetImageUrl(int taskId)
        {
            var response = new Response<string>();

            try
            {
                var task = await _dbContext.Base_StationTask_TightenByImages.FirstOrDefaultAsync(f => f.StationTaskId == taskId);
                if (task?.ImageUrl == null)
                {
                    response.Code = 500;
                    response.Message = $"未找到id为{taskId}图示拧紧配方详情或ImageUrl为空";
                    return response;
                }

                response.Data = task.ImageUrl;
                
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = ex.Message;
            }
            
            return response;
        }
    }
}
