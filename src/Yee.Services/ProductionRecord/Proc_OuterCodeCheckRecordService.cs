using AsZero.DbContexts;

using Yee.Entitys.DBEntity.ProductionRecords;

namespace Yee.Services.ProductionRecord
{
    public class Proc_outerCodeCheckRecordService
    {
        private readonly ILogger<Proc_outerCodeCheckRecordService> _logger;
        public readonly AsZeroDbContext _dBContext;
        private readonly IConfiguration _configuration;

        public Proc_outerCodeCheckRecordService(ILogger<Proc_outerCodeCheckRecordService> logger, AsZeroDbContext dBContext, IConfiguration configuration)
        {
            _logger = logger;
            _dBContext = dBContext;
            _configuration = configuration;
        }

        public async Task AddNewRecord(Proc_OuterCodeCheckRecord entity)
        {
            await _dBContext.Proc_OuterCodeCheckRecords.AddAsync(entity);
            await _dBContext.SaveChangesAsync();
        }

        public async Task<List<Proc_OuterCodeCheckRecord>> GetRecords(string PackCode)
        {
            
            var entiies = from record in _dBContext.Proc_OuterCodeCheckRecords
                          where !record.IsDeleted
                          select record;

            if(string.IsNullOrEmpty(PackCode))
            {
                entiies = entiies.Where(w => w.PackCode == PackCode);
            }
            return entiies.ToList();
        }

    }
}
