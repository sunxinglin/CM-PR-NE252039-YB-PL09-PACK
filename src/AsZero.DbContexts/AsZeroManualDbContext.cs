using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using Yee.Entitys.BaseData;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.Production;

namespace AsZero.DbContexts;

/// <summary>
/// 人工站专用：不再自动创建 不会使用到的表
/// </summary>
public sealed class AsZeroManualDbContext : AsZeroDbContext
{
    public AsZeroManualDbContext(DbContextOptions<AsZeroManualDbContext> options, IHostEnvironment env) : base(options, env)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<ActiveLastBoxType>();
        modelBuilder.Ignore<ModuleTypeTable>();
        modelBuilder.Ignore<CategoryType>();
        modelBuilder.Ignore<ActiveStationRelationship>();

        modelBuilder.Ignore<Base_AutoStationTaskGlue>();
        modelBuilder.Ignore<Base_StationTask_LowerBoxGlue>();
        modelBuilder.Ignore<Base_AutoStationTaskPressure>();
        modelBuilder.Ignore<Base_StationTask_AutoModuleInBox>();
        modelBuilder.Ignore<Base_AutoStationTask_HeatingFilmPressurize>();
        modelBuilder.Ignore<Base_Pack>();
        // modelBuilder.Ignore<Base_StationTaskStewingTime>();
        // modelBuilder.Ignore<Base_StationTask_RecordTime>();
        // modelBuilder.Ignore<Base_StationTaskCheckTimeOut>();

        modelBuilder.Ignore<Proc_GluingInfo>();
        modelBuilder.Ignore<Proc_PressureInfo>();
        modelBuilder.Ignore<Proc_ModuleInBox_GrapRecord>();
        modelBuilder.Ignore<Proc_ModuleInBox_DataCollect>();
        modelBuilder.Ignore<Proc_HeatingFilmPressurizeInfo>();
        modelBuilder.Ignore<Proc_LowerBoxGlueInfo>();
        // modelBuilder.Ignore<Proc_StationTask_StewingTime>();
        // modelBuilder.Ignore<Proc_StationTask_CheckTimeout>();
        // modelBuilder.Ignore<Proc_StationTask_PeiFang>();
    }
}
