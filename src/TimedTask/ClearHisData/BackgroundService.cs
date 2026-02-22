using AsZero.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Yee.Entitys.DBEntity;

namespace TimedTask.ClearHisData
{
    public class ClearHisDataService : BackgroundService
    {
        private readonly ILogger<ClearHisDataService> _logger;
        private readonly IOptionsMonitor<TimedTaskConifg> _TimedTaskConifg;
        private readonly IServiceProvider _service;
        public ClearHisDataService(ILogger<ClearHisDataService> logger, IOptionsMonitor<TimedTaskConifg> timedTaskConifg, IServiceProvider service)
        {
            _logger = logger;
            _TimedTaskConifg = timedTaskConifg;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //if (_TimedTaskConifg.CurrentValue.ClearHisData_Days < 92)
            //{
            //var thread = new Thread(() =>
            //{
            //    _ = RunAsync(stoppingToken, 92);
            //});
            //thread.IsBackground = true;
            //thread.Start();
            ////}
            //else
            //{
            //    var thread = new Thread(() =>
            //    {
            //        _ = RunAsync(stoppingToken, _TimedTaskConifg.CurrentValue.ClearHisData_Days);
            //    });
            //    thread.IsBackground = true;
            //    thread.Start();
            //}

        }

        private async Task RunAsync(CancellationToken ct, int days)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var time = DateTime.Now;
                    var CurrentHour = time.Hour;
                    var CurrentMinute = time.Minute;
                    if ((CurrentHour != 11 && CurrentHour != 23) || CurrentMinute < 10)
                    {
                        continue;
                    }
                    long TotolMilliseconds = ((long)_TimedTaskConifg.CurrentValue.ClearHisData_Days) * 24 * 60 * 60 * 1000;
                    long timePass = DateTimeOffset.Now.ToUnixTimeMilliseconds() - TotolMilliseconds;
                    var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timePass);
                    var dateTime = dateTimeOffset.DateTime;


                    using var scope = _service.CreateScope();
                    var sp = scope.ServiceProvider;
                    var _dbContext = sp.GetRequiredService<AsZeroDbContext>();
                    var mains = await _dbContext.Proc_StationTask_Mains.ToListAsync();

                    var deleteList = new List<Proc_StationTask_Main>();
                    foreach (var item in mains)
                    {
                        if (item.CreateTime != null && item.CreateTime < dateTime)
                        {
                            deleteList.Add(item);
                        }
                    }
                    _dbContext.RemoveRange(mains);
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    await Task.Delay(5 * 60 * 1000);
                    GC.Collect();
                }
            }


        }
    }
}
