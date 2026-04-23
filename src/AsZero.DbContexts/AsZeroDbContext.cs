using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using AsZero.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;

using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.BaseData;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.Common;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.Production;

namespace AsZero.DbContexts;

public partial class AsZeroDbContext : DbContext
{
    private readonly IHostEnvironment _env;

    public AsZeroDbContext(DbContextOptions<AsZeroDbContext> options, IHostEnvironment env) : base(options)
    {
        _env = env;
    }

    protected AsZeroDbContext(DbContextOptions options, IHostEnvironment env) : base(options)
    {
        _env = env;
    }

    public override int SaveChanges()
    {
        ApplyAuditFields();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditFields()
    {
        DateTime now = DateTime.Now;

        foreach (EntityEntry<BaseDataModel> entry in ChangeTracker.Entries<BaseDataModel>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreateTime ??= now;
                entry.Entity.UpdateTime = now;

                if (entry.Entity.IsDeleted)
                {
                    entry.Entity.DeleteTime ??= now;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdateTime = now;

                PropertyEntry isDeleted = entry.Property(nameof(BaseDataModel.IsDeleted));
                if (isDeleted.IsModified && entry.Entity.IsDeleted)
                {
                    entry.Entity.DeleteTime ??= now;
                }
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApiKeyEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new UserClaimEntityConfiguration());
        modelBuilder.ApplyConfiguration(new ResourceConfiguration());

        SeedDatabase(modelBuilder);
        if (_env.IsDevelopment())
        {
            SeedDatabaseForDevelopment(modelBuilder);
        }
    }


    #region 数据种子
    private static void SeedDatabase(ModelBuilder modelBuilder)
    {
        SeedUserAndClaims(modelBuilder);
        SeedResources(modelBuilder);
        SeedBaseDaTa(modelBuilder);
    }

    private static void SeedUserAndClaims(ModelBuilder modelBuilder)
    {
        # region 用户
        EntityTypeBuilder<User> userEntityType = modelBuilder.Entity<User>();
        // var userAdmin = new User { Id = 1, Account = "123456", Name = "超管123456", Password = "Pyq+0nRle0Dvj6VVf5lWZaRyXxk=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
        var userAdmin = new User { Id = 1, Account = "0", Name = "超管0", Password = "LTRkYQTt+/xYyGbhRkAdmKH2xLI=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
        var userAdminL = new User { Id = 5, Account = "l", Name = "超管l", Password = "B338L5yNtev0tOAEw5TG9xX1s7Q=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
        var userEngineer = new User { Id = 2, Account = Defines.UserAccount_DefaultEngineer, Name = "工程师", Password = "Pyq+0nRle0Dvj6VVf5lWZaRyXxk=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
        var userOperator = new User { Id = 3, Account = Defines.UserAccount_DefaultOperator, Name = "操作员", Password = "Pyq+0nRle0Dvj6VVf5lWZaRyXxk=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
        var userOperator2 = new User { Id = 4, Account = "0907312384", Name = "操作员", Password = "pRcLSFY0r2JGLrup61HxQemp7oE=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };

        userEntityType.HasData(userAdmin);
        userEntityType.HasData(userAdminL);
        userEntityType.HasData(userEngineer);
        userEntityType.HasData(userOperator);
        userEntityType.HasData(userOperator2);
        #endregion

        #region 权限
        EntityTypeBuilder<ClaimEntity> claimEntity = modelBuilder.Entity<ClaimEntity>();
        var claimAdmin = new ClaimEntity { Id = 1, ClaimType = ClaimTypes.Role, ClaimValue = Defines.Claim_Admin };
        var claimEngineer = new ClaimEntity { Id = 2, ClaimType = ClaimTypes.Role, ClaimValue = Defines.Claim_Engineer };
        var claimOperator = new ClaimEntity { Id = 3, ClaimType = ClaimTypes.Role, ClaimValue = Defines.Claim_Operator };
        var claimProductLinManage = new ClaimEntity { Id = 4, ClaimType = ClaimTypes.Role, ClaimValue = Defines.Claim_ProductLinManage };

        claimEntity.HasData(claimAdmin);
        claimEntity.HasData(claimEngineer);
        claimEntity.HasData(claimOperator);
        claimEntity.HasData(claimProductLinManage);
        #endregion

        #region  用户—权限
        var userclaimEntity = modelBuilder.Entity<UserClaim>();

        var userclaim1 = new UserClaim() { Id = 1, ClaimEntityId = 1, UserId = 1, };
        userclaimEntity.HasData(userclaim1);

        var userclaim5 = new UserClaim() { Id = 5, ClaimEntityId = 1, UserId = 5, };
        userclaimEntity.HasData(userclaim5);

        var userclaim2 = new UserClaim() { Id = 2, ClaimEntityId = 2, UserId = 2, };
        userclaimEntity.HasData(userclaim2);

        var userclaim3 = new UserClaim() { Id = 3, ClaimEntityId = 3, UserId = 3, };
        userclaimEntity.HasData(userclaim3);

        var userclaim4 = new UserClaim() { Id = 4, ClaimEntityId = 4, UserId = 4, };
        userclaimEntity.HasData(userclaim4);
        #endregion

        #region  网页端模块
        EntityTypeBuilder<FuncModule> moduleEntityType = modelBuilder.Entity<FuncModule>();
        moduleEntityType.HasData(new FuncModule { Id = 1, CascadeId = ".0.1.", Name = "系统配置", IconName = "jichupeizhi", Url = "/", ParentName = "根节点", SortNo = 1, ParentId = null, Code = null });
        moduleEntityType.HasData(new FuncModule { Id = 2, CascadeId = ".0.1.1.", Name = "模块管理", Url = "/ModuleManager/Index", ParentName = "基础配置", SortNo = 2, ParentId = 1, Code = "Module" });
        moduleEntityType.HasData(new FuncModule { Id = 3, CascadeId = ".0.1.2.", Name = "角色管理", IconName = "jiaoseguanli", Url = "/RoleManager/Index", ParentName = "基础配置", SortNo = 3, ParentId = 1, Code = "Role" });
        moduleEntityType.HasData(new FuncModule { Id = 4, CascadeId = ".0.1.3.", Name = "用户管理", IconName = "yonghuguanli", Url = "/UserManager/Index", ParentName = "基础配置", SortNo = 4, ParentId = 1, Code = "User" });
        moduleEntityType.HasData(new FuncModule { Id = 5, CascadeId = ".0.1.4.", Name = "数据字典", Url = "/Dictionary/Index", ParentName = "基础配置", SortNo = 5, ParentId = 1, Code = "Dictionary" });

        moduleEntityType.HasData(new FuncModule { Id = 201, CascadeId = ".0.2.", Name = "业务配置", Url = "/", ParentName = "根节点", SortNo = 1, ParentId = null, Code = null });
        moduleEntityType.HasData(new FuncModule { Id = 202, CascadeId = ".0.2.1.", Name = "产品", Url = "/Process/product/index", ParentName = "业务配置", SortNo = 2, ParentId = 201, Code = "Product" });
        moduleEntityType.HasData(new FuncModule { Id = 203, CascadeId = ".0.2.2.", Name = "工序", Url = "/Process/step/index", ParentName = "业务配置", SortNo = 3, ParentId = 201, Code = "Step" });
        moduleEntityType.HasData(new FuncModule { Id = 204, CascadeId = ".0.2.3.", Name = "工站", Url = "/Process/station/index", ParentName = "业务配置", SortNo = 4, ParentId = 201, Code = "Station" });
        moduleEntityType.HasData(new FuncModule { Id = 205, CascadeId = ".0.2.4.", Name = "工艺路线", Url = "/Process/flow/proflow", ParentName = "业务配置", SortNo = 5, ParentId = 201, Code = "ProFlow" });
        moduleEntityType.HasData(new FuncModule { Id = 206, CascadeId = ".0.2.5.", Name = "工艺配方", Url = "/Process/formula/index", ParentName = "业务配置", SortNo = 6, ParentId = 201, Code = "Formula" });
        moduleEntityType.HasData(new FuncModule { Id = 207, CascadeId = ".0.2.6.", Name = "生产资源", Url = "/Process/proresource/index", ParentName = "业务配置", SortNo = 7, ParentId = 201, Code = "ProResource" });
        moduleEntityType.HasData(new FuncModule { Id = 208, CascadeId = ".0.2.7.", Name = "拧紧NG管控", Url = "/tightenngresetconfig/index", ParentName = "业务配置", SortNo = 8, ParentId = 201, Code = "TightenNGResetCtrl" });

        moduleEntityType.HasData(new FuncModule { Id = 209, CascadeId = ".0.3.", Name = "AGV管理", Url = "/agvstatus/index", ParentName = "根节点", SortNo = 4, ParentId = null, Code = "agvstatus" });

        moduleEntityType.HasData(new FuncModule { Id = 210, CascadeId = ".0.3.", Name = "产出品下线", Url = "/", ParentName = "根节点", SortNo = 5, ParentId = null, Code = "productionrecord" });
        moduleEntityType.HasData(new FuncModule { Id = 211, CascadeId = ".0.3.1", Name = "下线列表", Url = "/productionrecord/production", ParentName = "产出品下线", SortNo = 1, ParentId = 210, Code = "productionrecordlist" });
        //moduleEntityType.HasData(new FuncModule { Id = 212, CascadeId = ".0.3.2", Name = "正常下线分析", Url = "/", ParentName = "产出品下线", SortNo = 2, ParentId = 210, Code = "OkDown" });
        //moduleEntityType.HasData(new FuncModule { Id = 213, CascadeId = ".0.3.3", Name = "NG下线分析", Url = "/", ParentName = "产出品下线", SortNo = 3, ParentId = 210, Code = "NGDown" });
        //moduleEntityType.HasData(new FuncModule { Id = 214, CascadeId = ".0.3.2.1", Name = "正常下线折线图", Url = "/productionrecord/okdown/brokenline", ParentName = "正常下线分析", SortNo = 1, ParentId = 212, Code = "okdownbrokenline" });
        //moduleEntityType.HasData(new FuncModule { Id = 215, CascadeId = ".0.3.2.2", Name = "正常下线圆饼图", Url = "/productionrecord/okdown/roundcake", ParentName = "正常下线分析", SortNo = 1, ParentId = 212, Code = "okdownroundcake" });
        //moduleEntityType.HasData(new FuncModule { Id = 216, CascadeId = ".0.3.3.1", Name = "NG下线折线图", Url = "/productionrecord/ngdown/brokenline", ParentName = "NG下线分析", SortNo = 1, ParentId = 213, Code = "ngdownbrokenline" });
        //moduleEntityType.HasData(new FuncModule { Id = 217, CascadeId = ".0.3.3.2", Name = "NG下线圆饼图", Url = "/productionrecord/ngdown/roundcake", ParentName = "NG下线分析", SortNo = 1, ParentId = 213, Code = "ngdownroundcake" });

        moduleEntityType.HasData(new FuncModule { Id = 218, CascadeId = ".0.4", Name = "追溯管理", IconName = "chaxun1", Url = "", ParentName = "追溯管理", SortNo = 6, ParentId = null, Code = "traceback" });
        moduleEntityType.HasData(new FuncModule { Id = 219, CascadeId = ".0.4.1", Name = "正向追溯", IconName = "chaxun", Url = "/traceback/forward", ParentName = "追溯管理", SortNo = 1, ParentId = 218, Code = "forward" });

        moduleEntityType.HasData(new FuncModule { Id = 220, CascadeId = ".0.5", Name = "拧紧数据管理", Url = "", ParentName = "拧紧枪数据管理", SortNo = 7, ParentId = null, Code = "blotgun" });
        moduleEntityType.HasData(new FuncModule { Id = 221, CascadeId = ".0.5.1", Name = "人工拧紧数据详情", Url = "/blotgun/blotgundetail", ParentName = "拧紧数据管理", SortNo = 1, ParentId = 220, Code = "blotgundetail" });
        // moduleEntityType.HasData(new FuncModule { Id = 222, CascadeId = ".0.5.2", Name = "自动拧紧数据详情", Url = "/blotgun/autoblotgundetail", ParentName = "拧紧数据管理", SortNo = 2, ParentId = 220, Code = "autoblotgundetail" });
        moduleEntityType.HasData(new FuncModule { Id = 223, CascadeId = ".0.5.3", Name = "自动拧紧数据详情", Url = "/blotgun/autotightenexternal", ParentName = "拧紧数据管理", SortNo = 3, ParentId = 220, Code = "autotightenexternal" });

        moduleEntityType.HasData(new FuncModule { Id = 225, CascadeId = ".0.6.", Name = "工位状态", IconName = "chaxun1", Url = "/statusquery/index", ParentName = "根节点", SortNo = 4, ParentId = null, Code = "StatusQuery" });

        // moduleEntityType.HasData(new FuncModule { Id = 228, CascadeId = ".0.7", Name = "涂胶数据管理", Url = "/", ParentName = "涂胶数据管理", SortNo = 7, ParentId = null, Code = "glue" });
        // moduleEntityType.HasData(new FuncModule { Id = 229, CascadeId = ".0.7.1", Name = "涂胶数据详情", Url = "/glue/gluedetail", ParentName = "涂胶数据管理", SortNo = 1, ParentId = 228, Code = "gluedetail" });
        // moduleEntityType.HasData(new FuncModule { Id = 231, CascadeId = ".0.7.2", Name = "下箱体涂胶数据详情", Url = "/glue/lowerboxglue", ParentName = "涂胶数据管理", SortNo = 1, ParentId = 228, Code = "lowerboxglue" });
        // moduleEntityType.HasData(new FuncModule { Id = 232, CascadeId = ".0.7.3", Name = "间隙涂胶数据详情", Url = "/glue/beamglue", ParentName = "涂胶数据管理", SortNo = 1, ParentId = 228, Code = "beamglue" });
        // moduleEntityType.HasData(new FuncModule { Id = 233, CascadeId = ".0.7.4", Name = "肩部涂胶数据详情", Url = "/glue/shoulderglue", ParentName = "涂胶数据管理", SortNo = 1, ParentId = 228, Code = "shoulderglue" });

        // moduleEntityType.HasData(new FuncModule { Id = 234, CascadeId = ".0.8", Name = "加压数据管理", Url = "/", ParentName = "加压数据管理", SortNo = 10, ParentId = null, Code = "press" });
        // moduleEntityType.HasData(new FuncModule { Id = 236, CascadeId = ".0.8.1", Name = "加压数据详情", Url = "/press/pressuresstrip", ParentName = "加压数据管理", SortNo = 1, ParentId = 234, Code = "pressuresstrip" });

        // moduleEntityType.HasData(new FuncModule { Id = 237, CascadeId = ".0.9", Name = "模组入箱管理", Url = "/", ParentName = "模组入箱管理", SortNo = 10, ParentId = null, Code = "moduleinbox" });
        // moduleEntityType.HasData(new FuncModule { Id = 238, CascadeId = ".0.9.1", Name = "模组入箱详情", Url = "/moduleinbox/datacollect", ParentName = "模组入箱管理", SortNo = 1, ParentId = 237, Code = "moduleinboxdatacollect" });

        moduleEntityType.HasData(new FuncModule { Id = 239, CascadeId = ".0.10", Name = "充气数据管理", Url = "/", ParentName = "充气数据管理", SortNo = 11, ParentId = null, Code = "leak" });
        moduleEntityType.HasData(new FuncModule { Id = 240, CascadeId = ".0.10.1", Name = "充气数据详情", Url = "/leak/leakdetail", ParentName = "充气数据管理", SortNo = 1, ParentId = 239, Code = "leakdetail" });


        moduleEntityType.HasData(new FuncModule { Id = 900, CascadeId = ".0.999.", Name = "日志管理", Url = "/", ParentName = "根节点", SortNo = 999, ParentId = null, Code = null });
        moduleEntityType.HasData(new FuncModule { Id = 901, CascadeId = ".0.999.1", Name = "系统日志", Url = "/syslogs/index", ParentName = "日志管理", SortNo = 999, ParentId = 900, Code = "SysLog" });
        moduleEntityType.HasData(new FuncModule { Id = 902, CascadeId = ".0.999.2", Name = "错误日志", Url = "/syslogs/alarm", ParentName = "日志管理", SortNo = 999, ParentId = 900, Code = "AlarmLog" });

        moduleEntityType.HasData(new FuncModule { Id = 1000, Name = "参数设置", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "ParamsMgmt" });
        moduleEntityType.HasData(new FuncModule { Id = 1001, Name = "AGV", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "AGV" });
        moduleEntityType.HasData(new FuncModule { Id = 1002, Name = "返工设置", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "ReWork" });
        moduleEntityType.HasData(new FuncModule { Id = 1003, Name = "NG下线", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "NGDown" });
        moduleEntityType.HasData(new FuncModule { Id = 1004, Name = "调试工具", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "DebugTool" });
        moduleEntityType.HasData(new FuncModule { Id = 1005, Name = "跳步", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "跳步" });
        moduleEntityType.HasData(new FuncModule { Id = 1006, Name = "拧紧NG复位", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "拧紧NG复位" });

        #endregion

        #region 网页端模块——用户权限 映射
        var funcModuleRoleMappingEntityType = modelBuilder.Entity<FuncModuleRoleMapping>();
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1, FuncModuleId = 1, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 2, FuncModuleId = 2, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 3, FuncModuleId = 3, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 4, FuncModuleId = 4, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 5, FuncModuleId = 5, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 201, FuncModuleId = 201, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 202, FuncModuleId = 202, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 203, FuncModuleId = 203, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 204, FuncModuleId = 204, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 205, FuncModuleId = 205, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 206, FuncModuleId = 206, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 207, FuncModuleId = 207, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 208, FuncModuleId = 208, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 209, FuncModuleId = 209, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 210, FuncModuleId = 210, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 211, FuncModuleId = 211, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 218, FuncModuleId = 218, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 219, FuncModuleId = 219, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 220, FuncModuleId = 220, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 221, FuncModuleId = 221, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 223, FuncModuleId = 223, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 225, FuncModuleId = 225, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 239, FuncModuleId = 239, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 240, FuncModuleId = 240, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 900, FuncModuleId = 900, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 901, FuncModuleId = 901, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 902, FuncModuleId = 902, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1000, FuncModuleId = 1000, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1001, FuncModuleId = 1001, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1002, FuncModuleId = 1002, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1003, FuncModuleId = 1003, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1004, FuncModuleId = 1004, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1005, FuncModuleId = 1005, RoleName = Defines.Claim_Admin });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1006, FuncModuleId = 1006, RoleName = Defines.Claim_Admin });

        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10001, FuncModuleId = 1, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10002, FuncModuleId = 2, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10004, FuncModuleId = 4, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10005, FuncModuleId = 5, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10201, FuncModuleId = 201, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10202, FuncModuleId = 202, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10203, FuncModuleId = 203, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10204, FuncModuleId = 204, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10205, FuncModuleId = 205, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10206, FuncModuleId = 206, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10207, FuncModuleId = 207, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10208, FuncModuleId = 208, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10209, FuncModuleId = 209, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10210, FuncModuleId = 210, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10211, FuncModuleId = 211, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10218, FuncModuleId = 218, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10219, FuncModuleId = 219, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10220, FuncModuleId = 220, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10221, FuncModuleId = 221, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10223, FuncModuleId = 223, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10225, FuncModuleId = 225, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10900, FuncModuleId = 900, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10901, FuncModuleId = 901, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 10902, FuncModuleId = 902, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 11000, FuncModuleId = 1000, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 11001, FuncModuleId = 1001, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 11002, FuncModuleId = 1002, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 11003, FuncModuleId = 1003, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 11004, FuncModuleId = 1004, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 11005, FuncModuleId = 1005, RoleName = Defines.Claim_Engineer });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 11006, FuncModuleId = 1006, RoleName = Defines.Claim_Engineer });

        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 20209, FuncModuleId = 209, RoleName = Defines.Claim_Operator });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 20225, FuncModuleId = 225, RoleName = Defines.Claim_Operator });
        funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 21002, FuncModuleId = 1002, RoleName = Defines.Claim_Operator });
        #endregion

    }

    private static void SeedResources(ModelBuilder modelBuilder)
    {
        var claimAdmin = new Claim(ClaimTypes.Role, Defines.Claim_Admin);
        var claimEngineer = new Claim(ClaimTypes.Role, Defines.Claim_Engineer);
        var claimOperator = new Claim(ClaimTypes.Role, Defines.Claim_Operator);

        EntityTypeBuilder<Resource> resourceEntity = modelBuilder.Entity<Resource>();
        var resourceRoot = new Resource
        {
            Id = 1,
            UniqueName = "",
            Description = "根资源",
            AllowedClaims = new List<Claim> { claimOperator, claimEngineer, claimAdmin },
            Configurable = false,
        };
        resourceEntity.HasData(resourceRoot);

        var resUserMgmt = new Resource
        {
            Id = 2,
            UniqueName = "用户管理",
            Description = "",
            AllowedClaims = new List<Claim> { claimOperator, claimEngineer, claimAdmin },
            ParentId = resourceRoot.Id,
            Configurable = false,
        };
        resourceEntity.HasData(resUserMgmt);

        var resUserMgmt_Login = new Resource
        {
            Id = 3,
            UniqueName = "用户登录",
            Description = "",
            AllowedClaims = new List<Claim> { claimEngineer, claimAdmin },
            ParentId = resUserMgmt.Id,
            Configurable = false,
        };
        resourceEntity.HasData(resUserMgmt_Login);

        var resUserMgmt_Logout = new Resource
        {
            Id = 4,
            UniqueName = "用户注销",
            Description = "",
            AllowedClaims = new List<Claim> { claimEngineer, claimAdmin },
            ParentId = resUserMgmt.Id,
            Configurable = false,
        };
        resourceEntity.HasData(resUserMgmt_Logout);

        var resUserMgmt_Maintain = new Resource
        {
            Id = 5,
            UniqueName = "用户维护",
            Description = "",
            AllowedClaims = new List<Claim> { claimEngineer, claimAdmin },
            ParentId = resUserMgmt.Id,
        };
        resourceEntity.HasData(resUserMgmt_Maintain);

        var resouceManagePrivi = new Resource
        {
            Id = 6,
            UniqueName = "权限管理",
            Description = "",
            AllowedClaims = new List<Claim> { claimEngineer, claimAdmin },
            ParentId = resUserMgmt.Id,
        };
        resourceEntity.HasData(resouceManagePrivi);

        var resouceMonitoring = new Resource
        {
            Id = 7,
            UniqueName = "自动监控",
            Description = "",
            AllowedClaims = new List<Claim> { claimOperator, claimEngineer, claimAdmin },
            ParentId = resourceRoot.Id,
        };
        resourceEntity.HasData(resouceMonitoring);

        var resouceSetParams = new Resource
        {
            Id = 8,
            UniqueName = "参数设置",
            Description = "",
            AllowedClaims = new List<Claim> { claimAdmin },
            ParentId = resourceRoot.Id,
        };
        resourceEntity.HasData(resouceSetParams);

        var resmodule_Maintain = new Resource
        {
            Id = 9,
            UniqueName = "模块管理",
            Description = "",
            AllowedClaims = new List<Claim> { claimAdmin },
            ParentId = resourceRoot.Id,
        };
        resourceEntity.HasData(resmodule_Maintain);

    }


    private static void SeedBaseDaTa(ModelBuilder modelBuilder)
    {
        # region AGV
        EntityTypeBuilder<Proc_AGVStatus> AGVEntityType = modelBuilder.Entity<Proc_AGVStatus>();
        for (var i = 1; i <= 80; i++)
        {
            var agv = new Proc_AGVStatus
            {
                Id = i,
                AGVNo = i,
                Behavior = 0,
                PackPN = string.Empty,
                ProductType = "1",
                StationCode = string.Empty
            };
            AGVEntityType.HasData(agv);
        }
        # endregion

        # region 字典
        EntityTypeBuilder<Dictionary> dictionaryEntity = modelBuilder.Entity<Dictionary>();
        var dictionary1 = new Dictionary { Id = 1, Code = "ProductType", Name = "产品类型", IsDeleted = false };
        dictionaryEntity.HasData(dictionary1);

        EntityTypeBuilder<DictionaryDetail> dictionaryDetailEntity = modelBuilder.Entity<DictionaryDetail>();
        var dictionaryDetail1 = new DictionaryDetail { Id = 1, Code = "Common", Name = "Common", DictionaryId = 1, Value = 1, IsDeleted = false };

        dictionaryDetailEntity.HasData(dictionaryDetail1);
        #endregion

        #region 工序

        EntityTypeBuilder<Base_Step> StepEntityType = modelBuilder.Entity<Base_Step>();
        var Step1 = new Base_Step { Id = 1, Code = "OP010", Name = "下箱体上线(手动)", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Step2 = new Base_Step { Id = 2, Code = "OP020", Name = "下箱体上线", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        var Step3 = new Base_Step { Id = 3, Code = "OP030", Name = "箱体预装1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step4 = new Base_Step { Id = 4, Code = "OP040", Name = "箱体预装2", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step5 = new Base_Step { Id = 5, Code = "OP050", Name = "箱体预装3", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step6 = new Base_Step { Id = 6, Code = "OP060", Name = "箱体预装零部件检测", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Step7 = new Base_Step { Id = 7, Code = "OP070", Name = "下箱体涂胶", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        // var Step8 = new Base_Step { Id = 8, Code = "OP080", Name = "模组入箱1", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        // var Step9 = new Base_Step { Id = 9, Code = "OP090", Name = "模组入箱2", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        // var Step10 = new Base_Step { Id = 10, Code = "OP100", Name = "容量测试", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        // var Step11 = new Base_Step { Id = 11, Code = "OP110", Name = "低压绝缘测试", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        // var Step12 = new Base_Step { Id = 12, Code = "OP120", Name = "极柱寻址&激光清洗", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        var Step13 = new Base_Step { Id = 13, Code = "OP130", Name = "CCS安装", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Step14 = new Base_Step { Id = 14, Code = "OP140", Name = "CCS安装零部件检测", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        // var Step15 = new Base_Step { Id = 15, Code = "OP150", Name = "BSB焊接", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        //var Step16 = new Base_Step { Id = 16, Code = "OP160", Name = "焊后自动检测", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        //var Step17 = new Base_Step { Id = 17, Code = "OP170", Name = "模组顶部绝缘膜", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        var Step18 = new Base_Step { Id = 18, Code = "OP180", Name = "标准人工位", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Step19 = new Base_Step { Id = 19, Code = "OP190", Name = "肩部涂胶", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        var Step20 = new Base_Step { Id = 20, Code = "OP200", Name = "标准人工位", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Step21 = new Base_Step { Id = 21, Code = "OP210", Name = "压条自动安装和拧紧", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        var Step22 = new Base_Step { Id = 22, Code = "OP220", Name = "低压连接1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step23 = new Base_Step { Id = 23, Code = "OP230", Name = "低压连接2", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step24 = new Base_Step { Id = 24, Code = "OP240", Name = "低压连接3", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step25 = new Base_Step { Id = 25, Code = "OP250", Name = "低压连接4", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step26 = new Base_Step { Id = 26, Code = "OP260", Name = "高压连接1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step27 = new Base_Step { Id = 27, Code = "OP270", Name = "高压连接2", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step28 = new Base_Step { Id = 28, Code = "OP280", Name = "高压连接3", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step29 = new Base_Step { Id = 29, Code = "OP290", Name = "高压连接4", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step30 = new Base_Step { Id = 30, Code = "OP300", Name = "高低压连接零部件检测1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step31 = new Base_Step { Id = 31, Code = "OP310", Name = "高低压连接零部件检测2", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step32 = new Base_Step { Id = 32, Code = "OP320", Name = "高低压连接零部件检测3", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step33 = new Base_Step { Id = 33, Code = "OP330", Name = "上盖预拧", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Step34 = new Base_Step { Id = 34, Code = "OP340", Name = "箱盖自动拧紧", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        var Step35 = new Base_Step { Id = 35, Code = "OP350", Name = "上盖补拧", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Step36 = new Base_Step { Id = 36, Code = "OP360", Name = "上盖补拧", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        // var Step37 = new Base_Step { Id = 37, Code = "OP370", Name = "高压人工位", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step38 = new Base_Step { Id = 38, Code = "OP380", Name = "拆工装+贴标", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Step39 = new Base_Step { Id = 39, Code = "OP390", Name = "拆工装+贴标", StepType = StepTypeEnum.线内人工站, IsDeleted = true };
        var Step40 = new Base_Step { Id = 40, Code = "OP400", Name = "Pack下线", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Step41 = new Base_Step { Id = 41, Code = "OP410", Name = "一体式集成站1", StepType = StepTypeEnum.线外人工站, IsDeleted = false };
        var Step42 = new Base_Step { Id = 42, Code = "OP420", Name = "一体式集成站2", StepType = StepTypeEnum.线外人工站, IsDeleted = false };
        var Step43 = new Base_Step { Id = 43, Code = "OP430", Name = "一体式集成站3", StepType = StepTypeEnum.线外人工站, IsDeleted = false };
        var Step44 = new Base_Step { Id = 44, Code = "OP440", Name = "返工拉下线", StepType = StepTypeEnum.线外人工站, IsDeleted = false };
        var Step45 = new Base_Step { Id = 45, Code = "OP450", Name = "返工拉人工位", StepType = StepTypeEnum.线外人工站, IsDeleted = false };

        StepEntityType.HasData(new Base_Step[]
        {
            Step1, Step3, Step4, Step5, Step6,
            Step13, Step18, Step20, Step22, Step23, Step24, Step25, Step26, Step27, Step28, Step29,
            Step30, Step31, Step32, Step33, Step35, Step38, Step40, Step41, Step42, Step43,
            Step44, Step45
        });

        #endregion

        #region 工站

        EntityTypeBuilder<Base_Station> StationEntityType = modelBuilder.Entity<Base_Station>();

        var Station1 = new Base_Station { Id = 1, StepId = 1, Code = "OP010", Name = "下箱体上线(手动)", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Station2 = new Base_Station { Id = 2, StepId = 2, Code = "OP020", Name = "标准人工站", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station3 = new Base_Station { Id = 3, StepId = 3, Code = "OP030", Name = "箱体预装1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station4 = new Base_Station { Id = 4, StepId = 4, Code = "OP040", Name = "箱体预装2", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station5 = new Base_Station { Id = 5, StepId = 5, Code = "OP050", Name = "箱体预装3", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station6 = new Base_Station { Id = 6, StepId = 6, Code = "OP060", Name = "箱体预装零部件检测", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Station7 = new Base_Station { Id = 7, StepId = 7, Code = "OP070", Name = "标准人工站", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Station8 = new Base_Station { Id = 8, StepId = 8, Code = "OP080", Name = "下箱体涂胶", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Station9 = new Base_Station { Id = 9, StepId = 9, Code = "OP090", Name = "涂胶检测", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Station10 = new Base_Station { Id = 10, StepId = 10, Code = "OP100", Name = "人工补胶", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Station11 = new Base_Station { Id = 11, StepId = 11, Code = "OP110", Name = "放行按钮", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Station12 = new Base_Station { Id = 12, StepId = 12, Code = "OP120", Name = "模组自动入箱1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station13 = new Base_Station { Id = 13, StepId = 13, Code = "OP130-1", Name = "CCS安装零部件检测A", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station14 = new Base_Station { Id = 14, StepId = 13, Code = "OP130-2", Name = "CCS安装零部件检测B", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Station15 = new Base_Station { Id = 15, StepId = 15, Code = "OP150", Name = "模组自动入箱4", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Station16 = new Base_Station { Id = 16, StepId = 16, Code = "OP160", Name = "放行按钮", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Station17 = new Base_Station { Id = 17, StepId = 17, Code = "OP170", Name = "低压绝缘测试", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station18 = new Base_Station { Id = 18, StepId = 18, Code = "OP180", Name = "标准人工位", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Station19 = new Base_Station { Id = 19, StepId = 19, Code = "OP190", Name = "CCS安装", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station20 = new Base_Station { Id = 20, StepId = 20, Code = "OP200", Name = "标准人工位", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        //var Station21 = new Base_Station { Id = 21, StepId = 21, Code = "OP210", Name = "放行按钮", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station22 = new Base_Station { Id = 22, StepId = 22, Code = "OP220", Name = "低压连接1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station23 = new Base_Station { Id = 23, StepId = 23, Code = "OP230", Name = "低压连接2", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station24 = new Base_Station { Id = 24, StepId = 24, Code = "OP240", Name = "低压连接3", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station25 = new Base_Station { Id = 25, StepId = 25, Code = "OP250", Name = "低压连接4", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station26 = new Base_Station { Id = 26, StepId = 26, Code = "OP260", Name = "高压连接1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station27 = new Base_Station { Id = 27, StepId = 27, Code = "OP270", Name = "高压连接2", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station28 = new Base_Station { Id = 28, StepId = 28, Code = "OP280", Name = "高压连接3", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station29 = new Base_Station { Id = 29, StepId = 29, Code = "OP290", Name = "高压连接4", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station30 = new Base_Station { Id = 30, StepId = 30, Code = "OP300", Name = "高低压零部件检测1", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station31 = new Base_Station { Id = 31, StepId = 31, Code = "OP310", Name = "高低压零部件检测2", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station32 = new Base_Station { Id = 32, StepId = 32, Code = "OP320", Name = "高低压零部件检测3", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station33 = new Base_Station { Id = 33, StepId = 33, Code = "OP330", Name = "高低压零部件检测4", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Station34 = new Base_Station { Id = 34, StepId = 33, Code = "OP340", Name = "标准人工位", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station35 = new Base_Station { Id = 35, StepId = 35, Code = "OP350-1", Name = "上盖补拧A", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station36 = new Base_Station { Id = 36, StepId = 35, Code = "OP350-2", Name = "上盖补拧B", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        // var Station37 = new Base_Station { Id = 37, StepId = 37, Code = "OP370", Name = "高压人工位", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station38 = new Base_Station { Id = 38, StepId = 38, Code = "OP380-1", Name = "拆工装+贴标A", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station39 = new Base_Station { Id = 39, StepId = 38, Code = "OP380-2", Name = "拆工装+贴标B", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station40 = new Base_Station { Id = 40, StepId = 40, Code = "OP400", Name = "Pack下线", StepType = StepTypeEnum.线内人工站, IsDeleted = false };
        var Station41 = new Base_Station { Id = 41, StepId = 41, Code = "OP410", Name = "一体式集成站1", StepType = StepTypeEnum.线外人工站, IsDeleted = false };
        var Station42 = new Base_Station { Id = 42, StepId = 42, Code = "OP420", Name = "一体式集成站2", StepType = StepTypeEnum.线外人工站, IsDeleted = false };
        var Station43 = new Base_Station { Id = 43, StepId = 43, Code = "OP430", Name = "一体式集成站3", StepType = StepTypeEnum.线外人工站, IsDeleted = false };
        var Station44 = new Base_Station { Id = 44, StepId = 44, Code = "OP440", Name = "返工拉下线", StepType = StepTypeEnum.线外人工站, IsDeleted = false };
        var Station45 = new Base_Station { Id = 45, StepId = 45, Code = "OP450", Name = "返工拉人工位", StepType = StepTypeEnum.线外人工站, IsDeleted = false };


        StationEntityType.HasData(new Base_Station[] {
            Station1, Station3, Station4, Station5, Station6,
            Station13, Station14, Station18, Station20,
            Station22, Station23, Station24, Station25, Station26,
            Station27, Station28, Station29,
            Station30, Station31, Station32, Station33,
            Station35, Station36, Station38, Station39, Station40, Station41, Station42, Station43,
            Station44, Station45
        });

        #endregion

        #region 生产资源

        EntityTypeBuilder<Base_ProResource> ProResourceEntityType = modelBuilder.Entity<Base_ProResource>();

        var ProResource1_1 = new Base_ProResource
        {
            Id = 1, IpAddress = "192.168.1.30", Name = "OP010拧紧枪", StationCode = "OP010",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource1_2 = new Base_ProResource
        {
            Id = 2, IpAddress = "", Name = "OP010扫码枪", StationCode = "OP010", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource3_1 = new Base_ProResource
        {
            Id = 3, IpAddress = "192.168.3.30", Name = "OP030拧紧枪", StationCode = "OP030",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource3_2 = new Base_ProResource
        {
            Id = 4, IpAddress = "", Name = "OP030扫码枪", StationCode = "OP030", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource4_1 = new Base_ProResource
        {
            Id = 5, IpAddress = "192.168.4.30", Name = "OP040拧紧枪", StationCode = "OP040",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource4_2 = new Base_ProResource
        {
            Id = 6, IpAddress = "", Name = "OP040扫码枪", StationCode = "OP040", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource5_1 = new Base_ProResource
        {
            Id = 7, IpAddress = "192.168.5.30", Name = "OP050拧紧枪", StationCode = "OP050",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource5_2 = new Base_ProResource
        {
            Id = 8, IpAddress = "", Name = "OP050扫码枪", StationCode = "OP050", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource6_1 = new Base_ProResource
        {
            Id = 9, IpAddress = "192.168.6.30", Name = "OP060拧紧枪", StationCode = "OP060",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource6_2 = new Base_ProResource
        {
            Id = 10, IpAddress = "", Name = "OP060扫码枪", StationCode = "OP060", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource13_1 = new Base_ProResource
        {
            Id = 11, IpAddress = "192.168.13.30", Name = "OP130-1拧紧枪", StationCode = "OP130-1",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource13_2 = new Base_ProResource
        {
            Id = 12, IpAddress = "", Name = "OP130-1扫码枪", StationCode = "OP130-1", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };
        var ProResource13_3 = new Base_ProResource
        {
            Id = 13, IpAddress = "192.168.13.32", Name = "OP130-2拧紧枪", StationCode = "OP130-2",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource13_4 = new Base_ProResource
        {
            Id = 14, IpAddress = "", Name = "OP130-2扫码枪", StationCode = "OP130-2", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource18_1 = new Base_ProResource
        {
            Id = 15, IpAddress = "192.168.18.30", Name = "OP180拧紧枪", StationCode = "OP180",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource18_2 = new Base_ProResource
        {
            Id = 16, IpAddress = "", Name = "OP180扫码枪", StationCode = "OP180", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource20_1 = new Base_ProResource
        {
            Id = 17, IpAddress = "192.168.20.30", Name = "OP200拧紧枪", StationCode = "OP200",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource20_2 = new Base_ProResource
        {
            Id = 18, IpAddress = "", Name = "OP200扫码枪", StationCode = "OP200", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource22_1 = new Base_ProResource
        {
            Id = 19, IpAddress = "192.168.22.30", Name = "OP220拧紧枪", StationCode = "OP220",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource22_2 = new Base_ProResource
        {
            Id = 20, IpAddress = "", Name = "OP220扫码枪", StationCode = "OP220", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource23_1 = new Base_ProResource
        {
            Id = 21, IpAddress = "192.168.23.30", Name = "OP230拧紧枪", StationCode = "OP230",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource23_2 = new Base_ProResource
        {
            Id = 22, IpAddress = "", Name = "OP230扫码枪", StationCode = "OP230", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource24_1 = new Base_ProResource
        {
            Id = 23, IpAddress = "192.168.24.30", Name = "OP240拧紧枪", StationCode = "OP240",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource24_2 = new Base_ProResource
        {
            Id = 24, IpAddress = "", Name = "OP240扫码枪", StationCode = "OP240", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource25_1 = new Base_ProResource
        {
            Id = 25, IpAddress = "192.168.25.30", Name = "OP250拧紧枪", StationCode = "OP250",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource25_2 = new Base_ProResource
        {
            Id = 26, IpAddress = "", Name = "OP250扫码枪", StationCode = "OP250", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource26_1 = new Base_ProResource
        {
            Id = 27, IpAddress = "192.168.26.30", Name = "OP260拧紧枪", StationCode = "OP260",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource26_2 = new Base_ProResource
        {
            Id = 28, IpAddress = "", Name = "OP260扫码枪", StationCode = "OP260", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource27_1 = new Base_ProResource
        {
            Id = 29, IpAddress = "192.168.27.30", Name = "OP270拧紧枪", StationCode = "OP270",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource27_2 = new Base_ProResource
        {
            Id = 30, IpAddress = "", Name = "OP270扫码枪", StationCode = "OP270", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource28_1 = new Base_ProResource
        {
            Id = 31, IpAddress = "192.168.28.30", Name = "OP280拧紧枪", StationCode = "OP280",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource28_2 = new Base_ProResource
        {
            Id = 32, IpAddress = "", Name = "OP280扫码枪", StationCode = "OP280", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource29_1 = new Base_ProResource
        {
            Id = 33, IpAddress = "192.168.29.30", Name = "OP290拧紧枪", StationCode = "OP290",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource29_2 = new Base_ProResource
        {
            Id = 34, IpAddress = "", Name = "OP290扫码枪", StationCode = "OP290", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource30_1 = new Base_ProResource
        {
            Id = 35, IpAddress = "192.168.30.30", Name = "OP300拧紧枪", StationCode = "OP300",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource30_2 = new Base_ProResource
        {
            Id = 36, IpAddress = "", Name = "OP300扫码枪", StationCode = "OP300", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource31_1 = new Base_ProResource
        {
            Id = 37, IpAddress = "192.168.31.30", Name = "OP310拧紧枪", StationCode = "OP310",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource31_2 = new Base_ProResource
        {
            Id = 38, IpAddress = "", Name = "OP310扫码枪", StationCode = "OP310", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource32_1 = new Base_ProResource
        {
            Id = 39, IpAddress = "192.168.32.30", Name = "OP320拧紧枪", StationCode = "OP320",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource32_2 = new Base_ProResource
        {
            Id = 40, IpAddress = "", Name = "OP320扫码枪", StationCode = "OP320", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource33_1 = new Base_ProResource
        {
            Id = 41, IpAddress = "192.168.33.30", Name = "OP330拧紧枪", StationCode = "OP330",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource33_2 = new Base_ProResource
        {
            Id = 42, IpAddress = "", Name = "OP330扫码枪", StationCode = "OP330", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource35_1 = new Base_ProResource
        {
            Id = 43, IpAddress = "192.168.35.30", Name = "OP350-1拧紧枪", StationCode = "OP350-1",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource35_2 = new Base_ProResource
        {
            Id = 44, IpAddress = "", Name = "OP350-1扫码枪", StationCode = "OP350-1", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };
        var ProResource35_3 = new Base_ProResource
        {
            Id = 45, IpAddress = "192.168.35.32", Name = "OP350-2拧紧枪", StationCode = "OP350-2",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource35_4 = new Base_ProResource
        {
            Id = 46, IpAddress = "", Name = "OP350-2扫码枪", StationCode = "OP350-2", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource38_1 = new Base_ProResource
        {
            Id = 47, IpAddress = "192.168.38.30", Name = "OP380-1拧紧枪", StationCode = "OP380-1",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource38_2 = new Base_ProResource
        {
            Id = 48, IpAddress = "", Name = "OP380-1扫码枪", StationCode = "OP380-1", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };
        var ProResource38_3 = new Base_ProResource
        {
            Id = 49, IpAddress = "192.168.38.32", Name = "OP380-2拧紧枪", StationCode = "OP380-2",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource38_4 = new Base_ProResource
        {
            Id = 50, IpAddress = "", Name = "OP380-2扫码枪", StationCode = "OP380-2", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource40_1 = new Base_ProResource
        {
            Id = 51, IpAddress = "192.168.40.30", Name = "OP400拧紧枪", StationCode = "OP400",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource40_2 = new Base_ProResource
        {
            Id = 52, IpAddress = "", Name = "OP400扫码枪", StationCode = "OP400", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource41_1 = new Base_ProResource
        {
            Id = 53, IpAddress = "192.168.41.30", Name = "OP410拧紧枪", StationCode = "OP410",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource41_2 = new Base_ProResource
        {
            Id = 54, IpAddress = "", Name = "OP410扫码枪", StationCode = "OP410", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource42_1 = new Base_ProResource
        {
            Id = 55, IpAddress = "192.168.42.30", Name = "OP420拧紧枪", StationCode = "OP420",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource42_2 = new Base_ProResource
        {
            Id = 56, IpAddress = "", Name = "OP420扫码枪", StationCode = "OP420", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource43_1 = new Base_ProResource
        {
            Id = 57, IpAddress = "192.168.43.30", Name = "OP430拧紧枪", StationCode = "OP430",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource43_2 = new Base_ProResource
        {
            Id = 58, IpAddress = "", Name = "OP430扫码枪", StationCode = "OP430", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource44_1 = new Base_ProResource
        {
            Id = 59, IpAddress = "192.168.44.30", Name = "OP440拧紧枪", StationCode = "OP440",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource44_2 = new Base_ProResource
        {
            Id = 60, IpAddress = "", Name = "OP440扫码枪", StationCode = "OP440", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource45_1 = new Base_ProResource
        {
            Id = 61, IpAddress = "192.168.45.30", Name = "OP450拧紧枪", StationCode = "OP450",
            ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545",
            DeviceNo = "1", IsDeleted = false, IsEnable = true, DeviceBrand = DeviceBrand.马头
        };
        var ProResource45_2 = new Base_ProResource
        {
            Id = 62, IpAddress = "", Name = "OP450扫码枪", StationCode = "OP450", ProResourceType = ProResourceTypeEnum.扫码枪,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.华睿
        };

        var ProResource50_3 = new Base_ProResource
        {
            Id = 65, IpAddress = "", Name = "OP500电子秤", StationCode = "OP500", ProResourceType = ProResourceTypeEnum.无线电子秤,
            ProtocolType = ProtocolTypeEnum.RS232, Port = "COM3", DeviceNo = "4", Baud = 4800, IsDeleted = false,
            IsEnable = true, DeviceBrand = DeviceBrand.Anyload
        };

        ProResourceEntityType.HasData(new Base_ProResource[]
        {
            ProResource1_1, ProResource1_2,
            ProResource3_1, ProResource3_2,
            ProResource4_1, ProResource4_2,
            ProResource5_1, ProResource5_2,
            ProResource6_1, ProResource6_2,
            ProResource13_1, ProResource13_2, ProResource13_3, ProResource13_4,
            ProResource18_1, ProResource18_2,
            ProResource20_1, ProResource20_2,
            ProResource22_1, ProResource22_2,
            ProResource23_1, ProResource23_2,
            ProResource24_1, ProResource24_2,
            ProResource25_1, ProResource25_2,
            ProResource26_1, ProResource26_2,
            ProResource27_1, ProResource27_2,
            ProResource28_1, ProResource28_2,
            ProResource29_1, ProResource29_2,
            ProResource30_1, ProResource30_2,
            ProResource31_1, ProResource31_2,
            ProResource32_1, ProResource32_2,
            ProResource33_1, ProResource33_2,
            ProResource35_1, ProResource35_2, ProResource35_3, ProResource35_4,
            ProResource38_1, ProResource38_2, ProResource38_3, ProResource38_4,
            ProResource40_1, ProResource40_2,
            ProResource41_1, ProResource41_2,
            ProResource42_1, ProResource42_2,
            ProResource43_1, ProResource43_2,
            ProResource44_1, ProResource44_2,
            ProResource45_1, ProResource45_2,
            ProResource50_3
        });

        #endregion

        # region 产品

        EntityTypeBuilder<Base_Product> productEntityType = modelBuilder.Entity<Base_Product>();
        var product00000 = new Base_Product
        {
            Id = 1,
            Code = "00000",
            Name = "00000",
            TypeId = 1,
            Specification = "00000",
            PackPNRule = "************************",
            PackOutCodeRule = "************",
            ModelRulesstr = "[]",
            Remark = "00000",
            IsDeleted = false
        };

        productEntityType.HasData(product00000);

        #endregion

        # region 工艺路线

        EntityTypeBuilder<Base_Flow> flowEntityType = modelBuilder.Entity<Base_Flow>();
        var flow00000 = new Base_Flow
        {
            Id = 1,
            Code = "00000",
            Name = "00000",
            Version = "00000",
            ProductId = product00000.Id,
            Description = "00000",
            Remark = "00000",
            IsDeleted = false
        };
        flowEntityType.HasData(flow00000);

        EntityTypeBuilder<Base_FlowStepMapping> flowStepMappingEntityType = modelBuilder.Entity<Base_FlowStepMapping>();
        var flowStepMapping00000 = new Base_FlowStepMapping
        {
            Id = 1,
            FlowId = flow00000.Id,
            StepId = 1,
            OrderNo = 1,
            Remark = "00000",
            IsDeleted = false
        };
        flowStepMappingEntityType.HasData(flowStepMapping00000);

        #endregion

        #region 工艺配方

        EntityTypeBuilder<Base_StationTask> stationTaskEntityType = modelBuilder.Entity<Base_StationTask>();
        var stationTaskScanAccountCard = new Base_StationTask
        {
            Id = 1,
            Code = "00000_ScanAccountCard",
            Name = "扫描员工卡",
            Type = StationTaskTypeEnum.扫描员工卡,
            HasPage = true,
            Clock = 30,
            StepId = 1,
            ProductId = product00000.Id,
            Sequence = 1,
            Description = "00000",
            Remark = "00000",
            IsDeleted = false
        };
        var stationTaskRelease = new Base_StationTask
        {
            Id = 2,
            Code = "00000_Release",
            Name = "放行",
            Type = StationTaskTypeEnum.放行,
            HasPage = true,
            Clock = 0,
            StepId = 1,
            ProductId = product00000.Id,
            Sequence = 2,
            Description = "00000",
            Remark = "00000",
            IsDeleted = false
        };
        stationTaskEntityType.HasData(stationTaskScanAccountCard, stationTaskRelease);

        EntityTypeBuilder<Base_StationTaskScanAccountCard> scanAccountCardEntityType = modelBuilder.Entity<Base_StationTaskScanAccountCard>();
        var scanAccountCard00000 = new Base_StationTaskScanAccountCard
        {
            Id = 1,
            ScanAccountCardName = "扫描员工卡",
            StationTaskId = stationTaskScanAccountCard.Id,
            UpMesCode = "YGK",
            Remark = "00000",
            IsDeleted = false
        };
        scanAccountCardEntityType.HasData(scanAccountCard00000);

        #endregion
    }

    private static void SeedDatabaseForDevelopment(ModelBuilder modelBuilder)
    {

    }

    #endregion
    public DbSet<ApiKey> ApiKeys { get; set; }

    #region 用户、模块
    public DbSet<User> Users { get; set; }
    public DbSet<ClaimEntity> Claims { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }

    public DbSet<FuncModule> FuncModules { get; set; }
    public DbSet<FuncModuleRoleMapping> FuncModuleRoleMappings { get; set; }

    #endregion


    #region 数据字典
    public DbSet<Dictionary> Dictionaries { get; set; }
    public DbSet<DictionaryDetail> DictionaryDetails { get; set; }

    #endregion


    #region 产品、工序
    public DbSet<Base_Product> Products { get; set; }
    public DbSet<Base_Flow> Base_Flows { get; set; }

    public DbSet<Base_Step> Base_Steps { get; set; }
    public DbSet<Base_FlowStepMapping> Base_FlowStepMappings { get; set; }
    public DbSet<Base_Station> Base_Stations { get; set; }

    public DbSet<Base_ProResource> Base_ProResources { get; set; }
    public DbSet<ActiveStationRelationship> ActiveStationRelationships { get; set; }
    public DbSet<ActiveLastBoxType> ActiveLastBoxTypes { get; set; }
    public DbSet<ModuleTypeTable> ModuleTypeTables { get; set; }
    #endregion

    #region 配方
    // 人工位
    public DbSet<Base_StationTask> Base_StationTasks { get; set; }
    public DbSet<Base_StationTaskCheckTimeOut> Base_StationTaskCheckTimeOuts { get; set; }
    public DbSet<Base_StationTaskBom> Base_StationTaskBoms { get; set; }
    public DbSet<Base_StationTaskAnyLoad> Base_StationTaskAnyLoads { get; set; }
    public DbSet<Base_StationTaskUserInput> Base_StationTaskUserInputs { get; set; }
    public DbSet<Base_StationTaskScanCollect> Base_StationTaskScanCollects { get; set; }
    public DbSet<Base_StationTaskScanAccountCard> Base_StationTaskScanAccountCards { get; set; }
    public DbSet<Base_StationTaskScrew> Base_StationTaskScrews { get; set; }
    public DbSet<Base_StationTask_RecordTime> Base_StationTask_RecordTimes { get; set; }
    public DbSet<Base_StationTask_TightenRework> Base_StationTask_TightenReworks { get; set; }
    public DbSet<Base_StationTask_TightenByImage> Base_StationTask_TightenByImages { get; set; }

    // 自动工位
    public DbSet<Base_AutoStationTaskGlue> Base_AutoStationTaskGlues { get; set; }
    public DbSet<Base_AutoStationTaskPressure> Base_AutoStationTaskPressures { get; set; }
    public DbSet<Base_StationTask_AutoModuleInBox> Base_StationTask_AutoModuleInBoxs { get; set; }
    public DbSet<Base_AutoStationTaskTighten> Base_AutoStationTaskTightens { get; set; }
    public DbSet<Base_StationTask_LowerBoxGlue> Base_StationTask_LowerBoxGlues { get; set; }
    public DbSet<Base_AutoStationTask_HeatingFilmPressurize> Base_AutoStationTask_HeatingFilmPressurizes { get; set; }
    public DbSet<Base_StationTaskStewingTime> Base_StationTaskStewingTimes { get; set; }

    #endregion

    #region BaseData
    public DbSet<CategoryType> CategoryTypes { get; set; }
    public DbSet<Base_Pack> Base_Packs { get; set; }
    #endregion

    #region 生产数据
    public DbSet<Proc_AGVStatus> Proc_AGVStatuss { get; set; }
    public DbSet<Proc_StationTask_Main> Proc_StationTask_Mains { get; set; }
    public DbSet<Proc_StationTask_Record> Proc_StationTask_Records { get; set; }
    public DbSet<Proc_StationTask_Bom> Proc_StationTask_Boms { get; set; }
    public DbSet<Proc_StationTask_BomDetail> Proc_StationTask_BomDetails { get; set; }
    public DbSet<Proc_StationTask_AnyLoad> Proc_StationTask_AnyLoads { get; set; }
    public DbSet<Proc_StationTask_ScanAccountCard> Proc_StationTask_ScanAccountCards { get; set; }
    public DbSet<Proc_StationTask_BlotGunDetail> Proc_StationTask_BlotGunDetails { get; set; }
    public DbSet<Proc_StationTask_BlotGun> Proc_StationTask_BlotGuns { get; set; }
    public DbSet<Proc_StationTask_UserInput> Proc_StationTask_UserInputs { get; set; }
    public DbSet<Proc_StationTask_ScanCollect> Proc_StationTask_ScanCollects { get; set; }
    public DbSet<Proc_StationTask_TimeRecord> Proc_StationTask_TimeRecords { get; set; }
    public DbSet<Proc_StationTask_TightenRework> Proc_StationTask_TightenReworks { get; set; }
    public DbSet<Proc_StationTask_TightenByImage> Proc_StationTask_TightenByImages { get; set; }
    public DbSet<Proc_OuterCodeCheckRecord> Proc_OuterCodeCheckRecords { get; set; }


    //自动工位
    public DbSet<Proc_GluingInfo> Proc_GluingInfos { get; set; }
    public DbSet<Proc_PressureInfo> Proc_PressureInfos { get; set; }
    public DbSet<Proc_AutoBoltInfo_Detail> Proc_AutoBoltInfo_Details { get; set; }
    public DbSet<Proc_ExternalAutoTightenData> Proc_ExternalAutoTightenDatas { get; set; }
    public DbSet<Proc_ModuleInBox_GrapRecord> Proc_ModuleInBox_GrapRecords { get; set; }
    public DbSet<Proc_ModuleInBox_DataCollect> Proc_ModuleInBox_DataCollects { get; set; }
    public DbSet<Proc_HeatingFilmPressurizeInfo> Proc_HeatingFilmPressurizeInfos { get; set; }
    public DbSet<Proc_LowerBoxGlueInfo> Proc_LowerBoxGlueInfos { get; set; }

    #endregion
    public DbSet<Proc_Product_Offline> Proc_Product_Offlines { get; set; }

    public DbSet<Proc_StationTask_CheckTimeout> Proc_StationTask_CheckTimeouts { get; set; }
    public DbSet<Proc_StationTask_StewingTime> Proc_StationTask_StewingTimes { get; set; }

    #region 刷卡记录
    public DbSet<Proc_CheckPowerRecord> Proc_CheckPowerRecords { get; set; }
    #endregion

    #region 历史配方
    public DbSet<Proc_StationTask_PeiFang> Proc_StationTask_PeiFangs { get; set; }
    #endregion

    #region 报警/日志
    public DbSet<Alarm> Alarms { get; set; }
    public DbSet<SysLog> Sys_Logs { get; set; }
    #endregion

    public DbSet<Base_StationTaskLeak> Base_StationTaskLeaks { get; set; }
    public DbSet<Proc_StationTask_Leak> Proc_StationTask_Leaks { get; set; }
    public DbSet<Proc_StationTask_LeakDetail> Proc_StationTask_LeakDetails { get; set; }

    # region 拧紧NG管控
    public DbSet<Proc_ScrewNGResetRecord> Proc_ScrewNGResetRecords { get; set; }
    public DbSet<Base_ScrewNGResetConfig> Base_ScrewNGResetConfigs { get; set; }
    #endregion


}
