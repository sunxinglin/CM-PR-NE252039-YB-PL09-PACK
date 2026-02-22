using AsZero.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Yee.Common.Library.CommonEnum;
using Yee.Entitys.AlarmMgmt;
using Yee.Entitys.BaseData;
using Yee.Entitys.DBEntity;
using Yee.Entitys.DBEntity.Production;
using Yee.Entitys.DBEntity.ProductionRecords;
using Yee.Entitys.Production;

namespace AsZero.DbContexts
{

    public partial class AsZeroDbContext : DbContext
    {
        private readonly IHostEnvironment _env;

        public AsZeroDbContext(DbContextOptions<AsZeroDbContext> options, IHostEnvironment env) : base(options)
        {
            this._env = env;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ApiKeyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ClaimEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserClaimEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ResouceConfiguration());
            #region  
            base.OnModelCreating(modelBuilder);

            #endregion

            SeedDatabase(modelBuilder);
            if (_env.IsDevelopment())
            {
                SeedDatabaseForDevelopment(modelBuilder);
            }
        }



        #region Seed Database
        private static void SeedDatabase(ModelBuilder modelBuilder)
        {
            SeedUserAndClaims(modelBuilder);
            SeedResouces(modelBuilder);
            SeedBaseDaTa(modelBuilder);
        }

        private static void SeedUserAndClaims(ModelBuilder modelBuilder)
        {
            // User
            var userEntityType = modelBuilder.Entity<User>();
            var userAdmin = new User { Id = 1, Account = "123456", Name = "管理员", Password = "Pyq+0nRle0Dvj6VVf5lWZaRyXxk=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
            var userEngineer = new User { Id = 2, Account = Defines.UserAccount_DefaultEngineer, Name = "工程师", Password = "Pyq+0nRle0Dvj6VVf5lWZaRyXxk=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
            var userOperator = new User { Id = 3, Account = Defines.UserAccount_DefaultOperator, Name = "操作员", Password = "Pyq+0nRle0Dvj6VVf5lWZaRyXxk=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
            var userOperator2 = new User { Id = 4, Account = "0907312384", Name = "操作员", Password = "pRcLSFY0r2JGLrup61HxQemp7oE=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
            var userOperator3 = new User { Id = 5, Account = "123456789012", Name = "操作员", Password = "aEx0 + J58J0kO5dzdwSxJTzydNlk =", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };
            var userOperator4 = new User { Id = 6, Account = "123456789013", Name = "操作员", Password = "mlo2pCaq/bcFj7K7+axClpsd0IU=", Salt = "ec0d0a12-70ff-4d93-b3e6-307403c99978", };

            userEntityType.HasData(userAdmin);
            userEntityType.HasData(userEngineer);
            userEntityType.HasData(userOperator);
            userEntityType.HasData(userOperator2);
            userEntityType.HasData(userOperator3);
            userEntityType.HasData(userOperator4);

            // Claim 
            #region claims
            var claimEntity = modelBuilder.Entity<ClaimEntity>();
            var claimAdmin = new ClaimEntity { Id = 1, ClaimType = ClaimTypes.Role, ClaimValue = Defines.Claim_Admin };
            var claimEngineer = new ClaimEntity { Id = 2, ClaimType = ClaimTypes.Role, ClaimValue = Defines.Claim_Engineer };
            var claimOperator = new ClaimEntity { Id = 3, ClaimType = ClaimTypes.Role, ClaimValue = Defines.Claim_Operator };
            var claimProductLinManage = new ClaimEntity { Id = 4, ClaimType = ClaimTypes.Role, ClaimValue = Defines.Claim_ProductLinManage };

            claimEntity.HasData(claimAdmin);
            claimEntity.HasData(claimEngineer);
            claimEntity.HasData(claimOperator);
            claimEntity.HasData(claimProductLinManage);
            #endregion

            #region  User-Claims
            var userclaimEntity = modelBuilder.Entity<UserClaim>();

            var userclaim1 = new UserClaim() { Id = 1, ClaimEntityId = 1, UserId = 1, };
            userclaimEntity.HasData(userclaim1);

            var userclaim2 = new UserClaim() { Id = 2, ClaimEntityId = 2, UserId = 2, };
            userclaimEntity.HasData(userclaim2);

            var userclaim3 = new UserClaim() { Id = 3, ClaimEntityId = 3, UserId = 3, };
            userclaimEntity.HasData(userclaim3);
            var userclaim4 = new UserClaim() { Id = 4, ClaimEntityId = 4, UserId = 4, };
            userclaimEntity.HasData(userclaim4);
            #endregion

            #region  func module
            var moduleEntityType = modelBuilder.Entity<FuncModule>();
            moduleEntityType.HasData(new FuncModule { Id = 1, CascadeId = ".0.1.", Name = "系统配置", Url = "/", ParentName = "根节点", SortNo = 1, ParentId = null, Code = null });
            moduleEntityType.HasData(new FuncModule { Id = 2, CascadeId = ".0.1.1.", Name = "模块管理", Url = "/ModuleManager/Index", ParentName = "基础配置", SortNo = 2, ParentId = 1, Code = "Module" });
            moduleEntityType.HasData(new FuncModule { Id = 3, CascadeId = ".0.1.2.", Name = "角色管理", Url = "/RoleManager/Index", ParentName = "基础配置", SortNo = 3, ParentId = 1, Code = "Role" });
            moduleEntityType.HasData(new FuncModule { Id = 4, CascadeId = ".0.1.3.", Name = "用户管理", Url = "/UserManager/Index", ParentName = "基础配置", SortNo = 4, ParentId = 1, Code = "User" });
            moduleEntityType.HasData(new FuncModule { Id = 5, CascadeId = ".0.1.4.", Name = "数据字典", Url = "/Dictionary/Index", ParentName = "基础配置", SortNo = 5, ParentId = 1, Code = "Dictionary" });


            moduleEntityType.HasData(new FuncModule { Id = 201, CascadeId = ".0.2.", Name = "业务配置", Url = "/", ParentName = "根节点", SortNo = 1, ParentId = null, Code = null });
            moduleEntityType.HasData(new FuncModule { Id = 202, CascadeId = ".0.2.1.", Name = "产品", Url = "/Process/product/index", ParentName = "业务配置", SortNo = 2, ParentId = 201, Code = "Product" });
            moduleEntityType.HasData(new FuncModule { Id = 203, CascadeId = ".0.2.2.", Name = "工艺路线", Url = "/Process/flow/proflow", ParentName = "业务配置", SortNo = 3, ParentId = 201, Code = "ProFlow" });
            //var moduleEntity204 = new FuncModule { Id = 204, CascadeId = ".0.2.3.", Name = "工位配方", Url = "/Process/station/Index", ParentName = "业务配置", SortNo = 4, ParentId = 201, Code = "Station" };
            moduleEntityType.HasData(new FuncModule { Id = 205, CascadeId = ".0.2.3.", Name = "生产资源", Url = "/Process/proresource/index", ParentName = "业务配置", SortNo = 5, ParentId = 201, Code = "ProResource" });
            moduleEntityType.HasData(new FuncModule { Id = 206, CascadeId = ".0.2.4.", Name = "配方管理", Url = "/Process/step/Index", ParentName = "业务配置", SortNo = 6, ParentId = 201, Code = "Step" });
            //var moduleEntity207 = new FuncModule { Id = 207, CascadeId = ".0.2.6.", Name = "工艺", Url = "/Process/flow/index", ParentName = "业务配置", SortNo = 9, ParentId = 2011, Code = "Flow" };


            moduleEntityType.HasData(new FuncModule { Id = 209, CascadeId = ".0.3.", Name = "AGV管理", Url = "/agvstatus/index", ParentName = "根节点", SortNo = 4, ParentId = null, Code = "agvstatus" });


            moduleEntityType.HasData(new FuncModule { Id = 210, CascadeId = ".0.3.", Name = "产出品下线", Url = "/", ParentName = "根节点", SortNo = 5, ParentId = null, Code = "productionrecord" });
            moduleEntityType.HasData(new FuncModule { Id = 211, CascadeId = ".0.3.1", Name = "下线列表", Url = "/productionrecord/production", ParentName = "产出品下线", SortNo = 1, ParentId = 210, Code = "productionrecordlist" });
            //moduleEntityType.HasData(new FuncModule { Id = 212, CascadeId = ".0.3.2", Name = "正常下线分析", Url = "/", ParentName = "产出品下线", SortNo = 2, ParentId = 210, Code = "OkDown" });
            //moduleEntityType.HasData(new FuncModule { Id = 213, CascadeId = ".0.3.3", Name = "NG下线分析", Url = "/", ParentName = "产出品下线", SortNo = 3, ParentId = 210, Code = "NGDown" });
            //moduleEntityType.HasData(new FuncModule { Id = 214, CascadeId = ".0.3.2.1", Name = "正常下线折线图", Url = "/productionrecord/okdown/brokenline", ParentName = "正常下线分析", SortNo = 1, ParentId = 212, Code = "okdownbrokenline" });
            //moduleEntityType.HasData(new FuncModule { Id = 215, CascadeId = ".0.3.2.2", Name = "正常下线圆饼图", Url = "/productionrecord/okdown/roundcake", ParentName = "正常下线分析", SortNo = 1, ParentId = 212, Code = "okdownroundcake" });
            //moduleEntityType.HasData(new FuncModule { Id = 216, CascadeId = ".0.3.3.1", Name = "NG下线折线图", Url = "/productionrecord/ngdown/brokenline", ParentName = "NG下线分析", SortNo = 1, ParentId = 213, Code = "ngdownbrokenline" });
            //moduleEntityType.HasData(new FuncModule { Id = 217, CascadeId = ".0.3.3.2", Name = "NG下线圆饼图", Url = "/productionrecord/ngdown/roundcake", ParentName = "NG下线分析", SortNo = 1, ParentId = 213, Code = "ngdownroundcake" });


            moduleEntityType.HasData(new FuncModule { Id = 218, CascadeId = ".0.4", Name = "追溯管理", Url = "", ParentName = "追溯管理", SortNo = 6, ParentId = null, Code = "traceback" });
            moduleEntityType.HasData(new FuncModule { Id = 219, CascadeId = ".0.4.1", Name = "正向追溯", Url = "/traceback/forward", ParentName = "追溯管理", SortNo = 1, ParentId = 218, Code = "forward" });

            moduleEntityType.HasData(new FuncModule { Id = 220, CascadeId = ".0.5", Name = "拧紧数据管理", Url = "", ParentName = "拧紧枪数据管理", SortNo = 7, ParentId = null, Code = "blotgun" });
            moduleEntityType.HasData(new FuncModule { Id = 221, CascadeId = ".0.5.1", Name = "人工拧紧数据详情", Url = "/blotgun/blotgundetail", ParentName = "拧紧数据管理", SortNo = 1, ParentId = 220, Code = "blotgundetail" });
            moduleEntityType.HasData(new FuncModule { Id = 227, CascadeId = ".0.5.2", Name = "自动拧紧数据详情", Url = "/blotgun/autoblotgundetail", ParentName = "拧紧数据管理", SortNo = 1, ParentId = 220, Code = "autoblotgundetail" });


            moduleEntityType.HasData(new FuncModule { Id = 228, CascadeId = ".0.7", Name = "涂胶数据管理", Url = "/", ParentName = "涂胶数据管理", SortNo = 7, ParentId = null, Code = "glue" });
            moduleEntityType.HasData(new FuncModule { Id = 229, CascadeId = ".0.7.1", Name = "涂胶数据详情", Url = "/glue/gluedetail", ParentName = "涂胶数据管理", SortNo = 1, ParentId = 228, Code = "gluedetail" });
            moduleEntityType.HasData(new FuncModule { Id = 231, CascadeId = ".0.7.2", Name = "下箱体涂胶数据详情", Url = "/glue/lowerboxglue", ParentName = "涂胶数据管理", SortNo = 1, ParentId = 228, Code = "lowerboxglue" });
            //moduleEntityType.HasData(new FuncModule { Id = 232, CascadeId = ".0.7.3", Name = "间隙涂胶数据详情", Url = "/glue/beamglue", ParentName = "涂胶数据管理", SortNo = 1, ParentId = 228, Code = "beamglue" });
            moduleEntityType.HasData(new FuncModule { Id = 233, CascadeId = ".0.7.4", Name = "肩部涂胶数据详情", Url = "/glue/shoulderglue", ParentName = "涂胶数据管理", SortNo = 1, ParentId = 228, Code = "shoulderglue" });


            moduleEntityType.HasData(new FuncModule { Id = 234, CascadeId = ".0.8", Name = "加压数据管理", Url = "/", ParentName = "加压数据管理", SortNo = 10, ParentId = null, Code = "press" });
            moduleEntityType.HasData(new FuncModule { Id = 236, CascadeId = ".0.8.1", Name = "加压数据详情", Url = "/press/pressuresstrip", ParentName = "加压数据管理", SortNo = 1, ParentId = 234, Code = "pressuresstrip" });


            moduleEntityType.HasData(new FuncModule { Id = 237, CascadeId = ".0.9", Name = "模组入箱管理", Url = "/", ParentName = "模组入箱管理", SortNo = 10, ParentId = null, Code = "moduleinbox" });
            moduleEntityType.HasData(new FuncModule { Id = 238, CascadeId = ".0.9.1", Name = "模组入箱详情", Url = "/moduleinbox/datacollect", ParentName = "模组入箱管理", SortNo = 1, ParentId = 237, Code = "moduleinboxdatacollect" });


            moduleEntityType.HasData(new FuncModule { Id = 900, CascadeId = ".0.999.", Name = "日志管理", Url = "/", ParentName = "根节点", SortNo = 999, ParentId = null, Code = null });
            moduleEntityType.HasData(new FuncModule { Id = 901, CascadeId = ".0.999.1", Name = "系统日志", Url = "/syslogs/index", ParentName = "日志管理", SortNo = 999, ParentId = 900, Code = "SysLog" });
            moduleEntityType.HasData(new FuncModule { Id = 902, CascadeId = ".0.999.2", Name = "错误日志", Url = "/syslogs/alarm", ParentName = "日志管理", SortNo = 999, ParentId = 900, Code = "AlarmLog" });


            moduleEntityType.HasData(new FuncModule { Id = 1000, Name = "客户端参数设置", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "ParamsMgmt" });
            moduleEntityType.HasData(new FuncModule { Id = 1001, Name = "AGV", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "AGV" });
            moduleEntityType.HasData(new FuncModule { Id = 1002, Name = "返工设置", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "ReWork" });
            moduleEntityType.HasData(new FuncModule { Id = 1003, Name = "NG下线", ParentName = "根节点", IsSys = false, SortNo = 1, ParentId = null, Code = "NGDown" });


            #endregion

            #region func module - mapping
            var funcModuleRoleMappingEntityType = modelBuilder.Entity<FuncModuleRoleMapping>();
            funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 1, FuncModuleId = 1, RoleName = Defines.Claim_Admin });
            funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 2, FuncModuleId = 2, RoleName = Defines.Claim_Admin });
            funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 3, FuncModuleId = 3, RoleName = Defines.Claim_Admin });
            funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 4, FuncModuleId = 1, RoleName = Defines.Claim_Engineer });
            funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 5, FuncModuleId = 2, RoleName = Defines.Claim_Engineer });
            funcModuleRoleMappingEntityType.HasData(new FuncModuleRoleMapping { Id = 6, FuncModuleId = 3, RoleName = Defines.Claim_Operator });
            #endregion


        }

        private static void SeedResouces(ModelBuilder modelBuilder)
        {
            var claimAdmin = new Claim(ClaimTypes.Role, Defines.Claim_Admin);
            var claimEngineer = new Claim(ClaimTypes.Role, Defines.Claim_Engineer);
            var claimOperator = new Claim(ClaimTypes.Role, Defines.Claim_Operator);

            var resouceEntity = modelBuilder.Entity<Resource>();
            var resouceRoot = new Resource
            {
                Id = 1,
                UniqueName = "",
                Description = "根资源",
                AllowedClaims = new List<Claim> { claimOperator, claimEngineer, claimAdmin },
                Configurable = false,
            };
            resouceEntity.HasData(resouceRoot);

            var resUserMgmt = new Resource
            {
                Id = 2,
                UniqueName = "用户管理",
                Description = "",
                AllowedClaims = new List<Claim> { claimOperator, claimEngineer, claimAdmin },
                ParentId = resouceRoot.Id,
                Configurable = false,
            };
            resouceEntity.HasData(resUserMgmt);

            var resUserMgmt_Login = new Resource
            {
                Id = 3,
                UniqueName = "用户登录",
                Description = "",
                AllowedClaims = new List<Claim> { claimEngineer, claimAdmin },
                ParentId = resUserMgmt.Id,
                Configurable = false,
            };
            resouceEntity.HasData(resUserMgmt_Login);

            var resUserMgmt_Logout = new Resource
            {
                Id = 4,
                UniqueName = "用户注销",
                Description = "",
                AllowedClaims = new List<Claim> { claimEngineer, claimAdmin },
                ParentId = resUserMgmt.Id,
                Configurable = false,
            };
            resouceEntity.HasData(resUserMgmt_Logout);

            var resUserMgmt_Maintain = new Resource
            {
                Id = 5,
                UniqueName = "用户维护",
                Description = "",
                AllowedClaims = new List<Claim> { claimEngineer, claimAdmin },
                ParentId = resUserMgmt.Id,
            };
            resouceEntity.HasData(resUserMgmt_Maintain);

            var resouceManagePrivi = new Resource
            {
                Id = 6,
                UniqueName = "权限管理",
                Description = "",
                AllowedClaims = new List<Claim> { claimEngineer, claimAdmin },
                ParentId = resUserMgmt.Id,
            };
            resouceEntity.HasData(resouceManagePrivi);

            var resouceMonitoring = new Resource
            {
                Id = 7,
                UniqueName = "自动监控",
                Description = "",
                AllowedClaims = new List<Claim> { claimOperator, claimEngineer, claimAdmin },
                ParentId = resouceRoot.Id,
            };
            resouceEntity.HasData(resouceMonitoring);

            var resouceSetParams = new Resource
            {
                Id = 8,
                UniqueName = "参数设置",
                Description = "",
                AllowedClaims = new List<Claim> { claimAdmin },
                ParentId = resouceRoot.Id,
            };
            resouceEntity.HasData(resouceSetParams);

            var resmodule_Maintain = new Resource
            {
                Id = 9,
                UniqueName = "模块管理",
                Description = "",
                AllowedClaims = new List<Claim> { claimAdmin },
                ParentId = resouceRoot.Id,
            };
            resouceEntity.HasData(resmodule_Maintain);

        }


        private static void SeedBaseDaTa(ModelBuilder modelBuilder)
        {
            // AGV
            var AGVEntityType = modelBuilder.Entity<Proc_AGVStatus>();
            var AGV1 = new Proc_AGVStatus { Id = 1, AGVNo = 1, Behavior = 0, PackPN = String.Empty, CreateTime = DateTime.Now, ProductType = "1", StationCode = String.Empty, IsDeleted = false };
            var AGV2 = new Proc_AGVStatus { Id = 2, AGVNo = 2, Behavior = 0, PackPN = String.Empty, CreateTime = DateTime.Now, ProductType = "1", StationCode = String.Empty, IsDeleted = false };
            var AGV3 = new Proc_AGVStatus { Id = 3, AGVNo = 3, Behavior = 0, PackPN = String.Empty, CreateTime = DateTime.Now, ProductType = "1", StationCode = String.Empty, IsDeleted = false };
            AGVEntityType.HasData(AGV1);
            AGVEntityType.HasData(AGV2);
            AGVEntityType.HasData(AGV3);

            //// Unit
            //var UnitEntityType = modelBuilder.Entity<Unit>();
            //var Unit1 = new Unit { Id = 1, Code = "Unit001", Name = "个", Type = 1, IsDeleted = false };
            //var Unit2 = new Unit { Id = 2, Code = "Unit002", Name = "台", Type = 1, IsDeleted = false };
            //UnitEntityType.HasData(Unit1);
            //UnitEntityType.HasData(Unit2);

            //Dictionary
            var DictionaryEntity = modelBuilder.Entity<Dictionary>();
            var Dictionary1 = new Dictionary { Id = 1, Code = "ProductType", Name = "产品类型", IsDeleted = false };
            DictionaryEntity.HasData(Dictionary1);

            var DictionaryDetailEntity = modelBuilder.Entity<DictionaryDetail>();
            var DictionaryDetail1 = new DictionaryDetail { Id = 1, Code = "Common", Name = "Common", DictionaryId = 1, Value = 1, IsDeleted = false };

            DictionaryDetailEntity.HasData(DictionaryDetail1);


            #region [Step]

            // Step
            var StepEntityType = modelBuilder.Entity<Base_Step>();
            var Step1 = new Base_Step { Id = 1, Code = "OP010", Name = "下箱体上线", IsDeleted = false };
            var Step2 = new Base_Step { Id = 2, Code = "OP020", Name = "标准人工站", IsDeleted = false };
            var Step3 = new Base_Step { Id = 3, Code = "OP030", Name = "标准人工站", IsDeleted = false };
            var Step4 = new Base_Step { Id = 4, Code = "OP040", Name = "标准人工站", IsDeleted = false };
            var Step5 = new Base_Step { Id = 5, Code = "OP050", Name = "加热膜加压", IsDeleted = false };
            var Step6 = new Base_Step { Id = 6, Code = "OP060", Name = "标准人工站", IsDeleted = false };
            var Step7 = new Base_Step { Id = 7, Code = "OP070", Name = "标准人工站", IsDeleted = false };
            var Step8 = new Base_Step { Id = 8, Code = "OP080", Name = "下箱体涂胶", IsDeleted = false };
            //var Step9 = new Base_Step { Id = 9, Code = "OP090", Name = "涂胶检测", IsDeleted = true };
            //var Step10 = new Base_Step { Id = 10, Code = "OP100", Name = "人工补胶", IsDeleted = true };
            //var Step11 = new Base_Step { Id = 11, Code = "OP110", Name = "放行按钮", IsDeleted = true };
            //var Step12 = new Base_Step { Id = 12, Code = "OP120", Name = "模组自动入箱1", IsDeleted = true };
            var Step13 = new Base_Step { Id = 13, Code = "OP130", Name = "模组自动入箱2", IsDeleted = false };
            var Step14 = new Base_Step { Id = 14, Code = "OP140", Name = "模组自动入箱3", IsDeleted = false };
            var Step15 = new Base_Step { Id = 15, Code = "OP150", Name = "模组自动入箱4", IsDeleted = false };
            //var Step16 = new Base_Step { Id = 16, Code = "OP160", Name = "放行按钮", IsDeleted = true };
            //var Step17 = new Base_Step { Id = 17, Code = "OP170", Name = "低压绝缘测试", IsDeleted = true };
            //var Step18 = new Base_Step { Id = 18, Code = "OP180", Name = "焊前寻址", IsDeleted = true };
            var Step19 = new Base_Step { Id = 19, Code = "OP190", Name = "CCS安装", IsDeleted = false };
            //var Step20 = new Base_Step { Id = 20, Code = "OP200", Name = "CCS安装", IsDeleted = false };
            //var Step21 = new Base_Step { Id = 21, Code = "OP210", Name = "放行按钮", IsDeleted = true };
            //var Step22 = new Base_Step { Id = 22, Code = "OP220", Name = "Busbar焊接", IsDeleted = true };
            //var Step23 = new Base_Step { Id = 23, Code = "OP230", Name = "焊后除尘", IsDeleted = true };
            //var Step24 = new Base_Step { Id = 24, Code = "OP240", Name = "放行按钮", IsDeleted = true };
            //var Step25 = new Base_Step { Id = 25, Code = "OP250", Name = "焊后检测", IsDeleted = true };
            //var Step26 = new Base_Step { Id = 26, Code = "OP260", Name = "肩部激光清洗", IsDeleted = true };
            //var Step27 = new Base_Step { Id = 27, Code = "OP270", Name = "贴绝缘膜", IsDeleted = false };
            var Step28 = new Base_Step { Id = 28, Code = "OP280", Name = "肩部涂胶", IsDeleted = false };
            var Step29 = new Base_Step { Id = 29, Code = "OP290", Name = "压条安装", IsDeleted = false };
            var Step30 = new Base_Step { Id = 30, Code = "OP300", Name = "压条自动加压", IsDeleted = false };

            var Step31 = new Base_Step { Id = 31, Code = "OP310", Name = "标准人工位", IsDeleted = false };
            //var Step32 = new Base_Step { Id = 32, Code = "OP320", Name = "标准人工位", IsDeleted = true };
            var Step33 = new Base_Step { Id = 33, Code = "OP330", Name = "标准人工位", IsDeleted = false };
            //var Step34 = new Base_Step { Id = 34, Code = "OP340", Name = "标准人工位", IsDeleted = true };
            var Step35 = new Base_Step { Id = 35, Code = "OP350", Name = "标准人工位", IsDeleted = false };
            //var Step36 = new Base_Step { Id = 36, Code = "OP360", Name = "标准人工位", IsDeleted = true };
            var Step37 = new Base_Step { Id = 37, Code = "OP370", Name = "高压人工位", IsDeleted = false };
            //var Step38 = new Base_Step { Id = 38, Code = "OP380", Name = "高压人工位", IsDeleted = true };
            var Step39 = new Base_Step { Id = 39, Code = "OP390", Name = "高压人工位", IsDeleted = false };
            //var Step40 = new Base_Step { Id = 40, Code = "OP400", Name = "高压人工位", IsDeleted = true };
            var Step41 = new Base_Step { Id = 41, Code = "OP410", Name = "高压人工位", IsDeleted = false };
            //var Step42 = new Base_Step { Id = 42, Code = "OP420", Name = "高压人工位", IsDeleted = true };
            var Step43 = new Base_Step { Id = 43, Code = "OP430", Name = "高压人工位", IsDeleted = false };
            //var Step44 = new Base_Step { Id = 44, Code = "OP440", Name = "高压人工位", IsDeleted = true };
            var Step45 = new Base_Step { Id = 45, Code = "OP450", Name = "标准人工位", IsDeleted = false };
            //var Step46 = new Base_Step { Id = 46, Code = "OP460", Name = "标准人工位", IsDeleted = true };
            var Step47 = new Base_Step { Id = 47, Code = "OP470", Name = "上盖预拧", IsDeleted = false };
            //var Step48 = new Base_Step { Id = 48, Code = "OP480", Name = "上盖预拧", IsDeleted = true };
            var Step49 = new Base_Step { Id = 49, Code = "OP490", Name = "上盖拧紧", IsDeleted = false };
            //var Step50 = new Base_Step { Id = 50, Code = "OP500", Name = "上盖拧紧", IsDeleted = true };
            var Step51 = new Base_Step { Id = 51, Code = "OP510", Name = "标准人工位", IsDeleted = false };
            //var Step52 = new Base_Step { Id = 52, Code = "OP520", Name = "标准人工位", IsDeleted = true };

            //var Step53 = new Base_Step { Id = 53, Code = "OP530", Name = "放行按钮", IsDeleted = true };
            //var Step54 = new Base_Step { Id = 54, Code = "OP540", Name = "放行按钮", IsDeleted = true };
            //var Step55 = new Base_Step { Id = 55, Code = "OP550", Name = "一体化测试", IsDeleted = true };
            var Step56 = new Base_Step { Id = 56, Code = "OP560", Name = "标准人工位", IsDeleted = false };
            //var Step57 = new Base_Step { Id = 57, Code = "OP570", Name = "放行按钮", IsDeleted = true };
            var Step58 = new Base_Step { Id = 58, Code = "OP580", Name = "Pack下线", IsDeleted = false };
            var Step59 = new Base_Step { Id = 59, Code = "OP590", Name = "SBOX1", IsDeleted = false };
            var Step60 = new Base_Step { Id = 60, Code = "OP600", Name = "SBOX2", IsDeleted = false };
            StepEntityType.HasData(new Base_Step[] {
                Step1, Step2, Step3, Step4, Step5, Step6, Step7, Step8,
                Step13, Step14, Step15,
                Step19,
                Step28, Step29, Step30,
                Step31,
                Step33,
                Step35,  Step37,  Step39,
                Step41, Step43,  Step45,  Step47,  Step49,
                Step51, Step56,  Step58, Step59, Step60,
            });

            #endregion

            #region [Station]

            // Station
            var StationEntityType = modelBuilder.Entity<Base_Station>();

            var Station1 = new Base_Station { Id = 1, StepId = 1, Code = "OP010", Name = "下箱体上线", IsDeleted = false };
            var Station2 = new Base_Station { Id = 2, StepId = 2, Code = "OP020", Name = "标准人工站", IsDeleted = false };
            var Station3 = new Base_Station { Id = 3, StepId = 3, Code = "OP030", Name = "标准人工站", IsDeleted = false };
            var Station4 = new Base_Station { Id = 4, StepId = 4, Code = "OP040", Name = "标准人工站", IsDeleted = false };
            var Station5 = new Base_Station { Id = 5, StepId = 5, Code = "OP050", Name = "加热膜加压", IsDeleted = false };
            var Station6 = new Base_Station { Id = 6, StepId = 6, Code = "OP060", Name = "标准人工站", IsDeleted = false };
            var Station7 = new Base_Station { Id = 7, StepId = 7, Code = "OP070", Name = "标准人工站", IsDeleted = false };
            var Station8 = new Base_Station { Id = 8, StepId = 8, Code = "OP080", Name = "下箱体涂胶", IsDeleted = false };
            //var Station9 = new Base_Station { Id = 9, StepId = 9, Code = "OP090", Name = "涂胶检测", IsDeleted = false };
            //var Station10 = new Base_Station { Id = 10, StepId = 10, Code = "OP100", Name = "人工补胶", IsDeleted = false };
            //var Station11 = new Base_Station { Id = 11, StepId = 11, Code = "OP110", Name = "放行按钮", IsDeleted = false };
            //var Station12 = new Base_Station { Id = 12, StepId = 12, Code = "OP120", Name = "模组自动入箱1", IsDeleted = false };
            var Station13 = new Base_Station { Id = 13, StepId = 13, Code = "OP130", Name = "模组自动入箱2", IsDeleted = false };
            var Station14 = new Base_Station { Id = 14, StepId = 14, Code = "OP140", Name = "模组自动入箱3", IsDeleted = false };
            var Station15 = new Base_Station { Id = 15, StepId = 15, Code = "OP150", Name = "模组自动入箱4", IsDeleted = false };
            //var Station16 = new Base_Station { Id = 16, StepId = 16, Code = "OP160", Name = "放行按钮", IsDeleted = false };
            //var Station17 = new Base_Station { Id = 17, StepId = 17, Code = "OP170", Name = "低压绝缘测试", IsDeleted = false };
            //var Station18 = new Base_Station { Id = 18, StepId = 18, Code = "OP180", Name = "焊前寻址", IsDeleted = false };
            var Station19 = new Base_Station { Id = 19, StepId = 19, Code = "OP190", Name = "CCS安装", IsDeleted = false };
            var Station20 = new Base_Station { Id = 20, StepId = 19, Code = "OP200", Name = "CCS安装", IsDeleted = false };
            //var Station21 = new Base_Station { Id = 21, StepId = 21, Code = "OP210", Name = "放行按钮", IsDeleted = false };
            //var Station22 = new Base_Station { Id = 22, StepId = 22, Code = "OP220", Name = "Busbar焊接", IsDeleted = false };
            //var Station23 = new Base_Station { Id = 23, StepId = 23, Code = "OP230", Name = "焊后除尘", IsDeleted = false };
            //var Station24 = new Base_Station { Id = 24, StepId = 24, Code = "OP240", Name = "放行按钮", IsDeleted = false };
            //var Station25 = new Base_Station { Id = 25, StepId = 25, Code = "OP250", Name = "焊后检测", IsDeleted = false };
            //var Station26 = new Base_Station { Id = 26, StepId = 26, Code = "OP260", Name = "肩部激光清洗", IsDeleted = false };
            //var Station27 = new Base_Station { Id = 27, StepId = 27, Code = "OP270", Name = "贴绝缘膜", IsDeleted = false };
            var Station28 = new Base_Station { Id = 28, StepId = 28, Code = "OP280", Name = "肩部涂胶", IsDeleted = false };
            var Station29 = new Base_Station { Id = 29, StepId = 29, Code = "OP290", Name = "压条安装", IsDeleted = false };
            var Station30 = new Base_Station { Id = 30, StepId = 30, Code = "OP300", Name = "压条自动加压", IsDeleted = false };

            var Station31 = new Base_Station { Id = 31, StepId = 31, Code = "OP310", Name = "标准人工位", IsDeleted = false };
            var Station32 = new Base_Station { Id = 32, StepId = 31, Code = "OP320", Name = "标准人工位", IsDeleted = false };
            var Station33 = new Base_Station { Id = 33, StepId = 33, Code = "OP330", Name = "标准人工位", IsDeleted = false };
            var Station34 = new Base_Station { Id = 34, StepId = 33, Code = "OP340", Name = "标准人工位", IsDeleted = false };
            var Station35 = new Base_Station { Id = 35, StepId = 35, Code = "OP350", Name = "标准人工位", IsDeleted = false };
            var Station36 = new Base_Station { Id = 36, StepId = 35, Code = "OP360", Name = "标准人工位", IsDeleted = false };
            var Station37 = new Base_Station { Id = 37, StepId = 37, Code = "OP370", Name = "高压人工位", IsDeleted = false };
            var Station38 = new Base_Station { Id = 38, StepId = 37, Code = "OP380", Name = "高压人工位", IsDeleted = false };
            var Station39 = new Base_Station { Id = 39, StepId = 39, Code = "OP390", Name = "高压人工位", IsDeleted = false };
            var Station40 = new Base_Station { Id = 40, StepId = 39, Code = "OP400", Name = "高压人工位", IsDeleted = false };
            var Station41 = new Base_Station { Id = 41, StepId = 41, Code = "OP410", Name = "高压人工位", IsDeleted = false };
            var Station42 = new Base_Station { Id = 42, StepId = 41, Code = "OP420", Name = "高压人工位", IsDeleted = false };
            var Station43 = new Base_Station { Id = 43, StepId = 43, Code = "OP430", Name = "高压人工位", IsDeleted = false };
            var Station44 = new Base_Station { Id = 44, StepId = 43, Code = "OP440", Name = "高压人工位", IsDeleted = false };
            var Station45 = new Base_Station { Id = 45, StepId = 45, Code = "OP450", Name = "标准人工位", IsDeleted = false };
            var Station46 = new Base_Station { Id = 46, StepId = 45, Code = "OP460", Name = "标准人工位", IsDeleted = false };
            var Station47 = new Base_Station { Id = 47, StepId = 47, Code = "OP470", Name = "上盖预拧", IsDeleted = false };
            var Station48 = new Base_Station { Id = 48, StepId = 47, Code = "OP480", Name = "上盖预拧", IsDeleted = false };
            var Station49 = new Base_Station { Id = 49, StepId = 49, Code = "OP490", Name = "上盖拧紧", IsDeleted = false };
            var Station50 = new Base_Station { Id = 50, StepId = 49, Code = "OP500", Name = "上盖拧紧", IsDeleted = false };
            var Station51 = new Base_Station { Id = 51, StepId = 51, Code = "OP510", Name = "标准人工位", IsDeleted = false };
            var Station52 = new Base_Station { Id = 52, StepId = 51, Code = "OP520", Name = "标准人工位", IsDeleted = false };

            //var Station53 = new Base_Station { Id = 53, StepId = 53, Code = "OP530", Name = "放行按钮", IsDeleted = false };
            //var Station54 = new Base_Station { Id = 54, StepId = 54, Code = "OP540", Name = "放行按钮", IsDeleted = false };
            //var Station55 = new Base_Station { Id = 55, StepId = 55, Code = "OP550", Name = "一体化测试", IsDeleted = false };
            var Station56 = new Base_Station { Id = 56, StepId = 56, Code = "OP560", Name = "标准人工位", IsDeleted = false };
            //var Station57 = new Base_Station { Id = 57, StepId = 57, Code = "OP570", Name = "放行按钮", IsDeleted = false };
            var Station58 = new Base_Station { Id = 58, StepId = 58, Code = "OP580", Name = "Pack下线", IsDeleted = false };
            var Station59 = new Base_Station { Id = 59, StepId = 59, Code = "OP590", Name = "SBOX1", IsDeleted = false };
            var Station60 = new Base_Station { Id = 60, StepId = 60, Code = "OP600", Name = "SBOX2", IsDeleted = false };

            StationEntityType.HasData(new Base_Station[] {
                Station1, Station2, Station3, Station4, Station5, Station6, Station7, Station8,
                 Station13, Station14, Station15, Station19, Station20, Station28, Station29, Station30,
                Station31, Station32, Station33, Station34, Station35, Station36, Station37, Station38, Station39, Station40,
                Station41, Station42, Station43, Station44, Station45, Station46, Station47, Station48, Station49, Station50,
                Station51, Station52,  Station56,  Station58, Station59, Station60,
            });

            #endregion

            #region [ProResource]

            // ProResource
            var ProResourceEntityType = modelBuilder.Entity<Base_ProResource>();

            var ProResource1_2 = new Base_ProResource { Id = 1, IpAddress = "192.168.10.102", Name = "OP010拧紧枪", StationCode = "OP010", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false,DeviceBrand = DeviceBrand.马头 };
            var ProResource1_3 = new Base_ProResource { Id = 2, IpAddress = "", Name = "OP010扫码枪", StationCode = "OP010", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource2_2 = new Base_ProResource { Id = 3, IpAddress = "192.168.20.102", Name = "OP020拧紧枪", StationCode = "OP020", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource2_3 = new Base_ProResource { Id = 4, IpAddress = "", Name = "OP020扫码枪", StationCode = "OP020", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource3_2 = new Base_ProResource { Id = 5, IpAddress = "192.168.30.102", Name = "OP030拧紧枪", StationCode = "OP030", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource3_3 = new Base_ProResource { Id = 6, IpAddress = "", Name = "OP030扫码枪", StationCode = "OP030", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource4_2 = new Base_ProResource { Id = 7, IpAddress = "192.168.40.102", Name = "OP040拧紧枪", StationCode = "OP040", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource4_3 = new Base_ProResource { Id = 8, IpAddress = "", Name = "OP040扫码枪", StationCode = "OP040", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource5_2 = new Base_ProResource { Id = 9, IpAddress = "192.168.60.102", Name = "OP060拧紧枪", StationCode = "OP060", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource5_3 = new Base_ProResource { Id = 10, IpAddress = "", Name = "OP060扫码枪", StationCode = "OP060", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource6_2 = new Base_ProResource { Id = 11, IpAddress = "192.168.20.102", Name = "OP070拧紧枪", StationCode = "OP070", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource6_3 = new Base_ProResource { Id = 12, IpAddress = "", Name = "OP070扫码枪", StationCode = "OP070", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource8_2 = new Base_ProResource { Id = 15, IpAddress = "192.168.20.102", Name = "OP190拧紧枪", StationCode = "OP190", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource8_3 = new Base_ProResource { Id = 16, IpAddress = "", Name = "OP190扫码枪", StationCode = "OP190", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource9_2 = new Base_ProResource { Id = 17, IpAddress = "192.168.10.102", Name = "OP200拧紧枪", StationCode = "OP200", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource9_3 = new Base_ProResource { Id = 18, IpAddress = "", Name = "OP200扫码枪", StationCode = "OP200", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource10_2 = new Base_ProResource { Id = 19, IpAddress = "192.168.20.102", Name = "OP290拧紧枪", StationCode = "OP290", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource10_3 = new Base_ProResource { Id = 20, IpAddress = "", Name = "OP290扫码枪", StationCode = "OP290", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource11_2 = new Base_ProResource { Id = 21, IpAddress = "192.168.30.102", Name = "OP310拧紧枪", StationCode = "OP310", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource11_3 = new Base_ProResource { Id = 22, IpAddress = "", Name = "OP310扫码枪", StationCode = "OP310", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource12_2 = new Base_ProResource { Id = 23, IpAddress = "192.168.40.102", Name = "OP320拧紧枪", StationCode = "OP320", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource12_3 = new Base_ProResource { Id = 24, IpAddress = "", Name = "OP320扫码枪", StationCode = "OP320", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource13_2 = new Base_ProResource { Id = 25, IpAddress = "192.168.60.102", Name = "OP330拧紧枪", StationCode = "OP330", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource13_3 = new Base_ProResource { Id = 26, IpAddress = "", Name = "OP330扫码枪", StationCode = "OP330", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource14_2 = new Base_ProResource { Id = 27, IpAddress = "192.168.20.102", Name = "OP340拧紧枪", StationCode = "OP340", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource14_3 = new Base_ProResource { Id = 28, IpAddress = "", Name = "OP340扫码枪", StationCode = "OP340", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource15_2 = new Base_ProResource { Id = 29, IpAddress = "192.168.10.102", Name = "OP350拧紧枪", StationCode = "OP350", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource15_3 = new Base_ProResource { Id = 30, IpAddress = "", Name = "OP350扫码枪", StationCode = "OP350", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource16_2 = new Base_ProResource { Id = 31, IpAddress = "192.168.20.102", Name = "OP360拧紧枪", StationCode = "OP360", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource16_3 = new Base_ProResource { Id = 32, IpAddress = "", Name = "OP360扫码枪", StationCode = "OP360", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource17_2 = new Base_ProResource { Id = 33, IpAddress = "192.168.10.102", Name = "OP370拧紧枪", StationCode = "OP370", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource17_3 = new Base_ProResource { Id = 34, IpAddress = "", Name = "OP370扫码枪", StationCode = "OP370", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource18_2 = new Base_ProResource { Id = 35, IpAddress = "192.168.20.102", Name = "OP380拧紧枪", StationCode = "OP380", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource18_3 = new Base_ProResource { Id = 36, IpAddress = "", Name = "OP380扫码枪", StationCode = "OP380", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource19_2 = new Base_ProResource { Id = 37, IpAddress = "192.168.30.102", Name = "OP390拧紧枪", StationCode = "OP390", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource19_3 = new Base_ProResource { Id = 38, IpAddress = "", Name = "OP390扫码枪", StationCode = "OP390", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource20_2 = new Base_ProResource { Id = 39, IpAddress = "192.168.40.102", Name = "OP400拧紧枪", StationCode = "OP400", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource20_3 = new Base_ProResource { Id = 40, IpAddress = "", Name = "OP400扫码枪", StationCode = "OP400", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource21_2 = new Base_ProResource { Id = 41, IpAddress = "192.168.60.102", Name = "OP410拧紧枪", StationCode = "OP410", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource21_3 = new Base_ProResource { Id = 42, IpAddress = "", Name = "OP410扫码枪", StationCode = "OP410", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource22_2 = new Base_ProResource { Id = 43, IpAddress = "192.168.20.102", Name = "OP420拧紧枪", StationCode = "OP420", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource22_3 = new Base_ProResource { Id = 44, IpAddress = "", Name = "OP420扫码枪", StationCode = "OP420", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource23_2 = new Base_ProResource { Id = 45, IpAddress = "192.168.10.102", Name = "OP430拧紧枪", StationCode = "OP430", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource23_3 = new Base_ProResource { Id = 46, IpAddress = "", Name = "OP430扫码枪", StationCode = "OP430", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource24_2 = new Base_ProResource { Id = 47, IpAddress = "192.168.20.102", Name = "OP440拧紧枪", StationCode = "OP440", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource24_3 = new Base_ProResource { Id = 48, IpAddress = "", Name = "OP440扫码枪", StationCode = "OP440", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };


            var ProResource25_2 = new Base_ProResource { Id = 49, IpAddress = "192.168.40.102", Name = "OP450拧紧枪", StationCode = "OP450", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource25_3 = new Base_ProResource { Id = 50, IpAddress = "", Name = "OP450扫码枪", StationCode = "OP450", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource26_2 = new Base_ProResource { Id = 51, IpAddress = "192.168.60.102", Name = "OP460拧紧枪", StationCode = "OP460", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource26_3 = new Base_ProResource { Id = 52, IpAddress = "", Name = "OP460扫码枪", StationCode = "OP460", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource27_2 = new Base_ProResource { Id = 53, IpAddress = "192.168.20.102", Name = "OP470拧紧枪", StationCode = "OP470", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource27_3 = new Base_ProResource { Id = 54, IpAddress = "", Name = "OP470扫码枪", StationCode = "OP470", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource28_2 = new Base_ProResource { Id = 55, IpAddress = "192.168.10.102", Name = "OP480拧紧枪", StationCode = "OP480", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource28_3 = new Base_ProResource { Id = 56, IpAddress = "", Name = "OP480扫码枪", StationCode = "OP480", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };


            var ProResource30_2 = new Base_ProResource { Id = 59, IpAddress = "192.168.20.102", Name = "OP510拧紧枪", StationCode = "OP510", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource30_3 = new Base_ProResource { Id = 60, IpAddress = "", Name = "OP510扫码枪", StationCode = "OP510", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };


            var ProResource31_2 = new Base_ProResource { Id = 61, IpAddress = "192.168.60.102", Name = "OP520拧紧枪", StationCode = "OP520", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource31_3 = new Base_ProResource { Id = 62, IpAddress = "", Name = "OP520扫码枪", StationCode = "OP520", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource32_2 = new Base_ProResource { Id = 63, IpAddress = "192.168.20.102", Name = "OP560拧紧枪", StationCode = "OP560", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource32_3 = new Base_ProResource { Id = 64, IpAddress = "", Name = "OP560扫码枪", StationCode = "OP560", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource33_2 = new Base_ProResource { Id = 65, IpAddress = "192.168.10.102", Name = "OP580拧紧枪", StationCode = "OP580", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource33_3 = new Base_ProResource { Id = 66, IpAddress = "", Name = "OP580扫码枪", StationCode = "OP580", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            var ProResource34_2 = new Base_ProResource { Id = 67, IpAddress = "192.168.20.102", Name = "OP590拧紧枪", StationCode = "OP590", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource34_3 = new Base_ProResource { Id = 68, IpAddress = "", Name = "OP590扫码枪", StationCode = "OP590", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };


            var ProResource35_2 = new Base_ProResource { Id = 69, IpAddress = "192.168.20.102", Name = "OP600拧紧枪", StationCode = "OP600", ProResourceType = ProResourceTypeEnum.拧紧枪, ProtocolType = ProtocolTypeEnum.TCPIP, Port = "4545", DeviceNo = "1", IsDeleted = false, IsEnable = false, DeviceBrand = DeviceBrand.马头 };
            var ProResource35_3 = new Base_ProResource { Id = 70, IpAddress = "", Name = "OP600扫码枪", StationCode = "OP600", ProResourceType = ProResourceTypeEnum.扫码枪, ProtocolType = ProtocolTypeEnum.RS232, Port = "COM4", DeviceNo = "3", Baud = 9600, IsDeleted = false, IsEnable = true };

            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource1_2, ProResource1_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource2_2, ProResource2_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource3_2, ProResource3_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource4_2, ProResource4_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource5_2, ProResource5_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource6_2, ProResource6_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource8_2, ProResource8_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource9_2, ProResource9_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource10_2, ProResource10_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource11_2, ProResource11_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource12_2, ProResource12_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource13_2, ProResource13_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource14_2, ProResource14_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource15_2, ProResource15_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource16_2, ProResource16_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource17_2, ProResource17_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource18_2, ProResource18_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource19_2, ProResource19_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource20_2, ProResource20_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource21_2, ProResource21_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource22_2, ProResource22_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource23_2, ProResource23_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource24_2, ProResource24_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource25_2, ProResource25_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource26_2, ProResource26_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource27_2, ProResource27_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource28_2, ProResource28_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource30_2, ProResource30_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource31_2, ProResource31_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource32_2, ProResource32_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource33_2, ProResource33_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource34_2, ProResource34_3 });
            ProResourceEntityType.HasData(new Base_ProResource[] { ProResource35_2, ProResource35_3 });

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

        // 自动工位
        public DbSet<Base_AutoStationTaskGlue> Base_AutoStationTaskGlues { get; set; }
        public DbSet<Base_AutoStationTaskPressure> Base_AutoStationTaskPressures { get; set; }
        public DbSet<Base_StationTask_AutoModuleInBox> Base_StationTask_AutoModuleInBoxs { get; set; }
        public DbSet<Base_AutoStationTaskTighten> Base_AutoStationTaskTightens { get; set; }

        public DbSet<Base_StationTask_LowerBoxGlue> Base_StationTask_LowerBoxGlues { get; set; }

        public DbSet<Base_AutoStationTask_HeatingFilmPressurize> Base_AutoStationTask_HeatingFilmPressurizes { get; set; }

        #endregion

        public DbSet<Base_StationTaskStewingTime> Base_StationTaskStewingTimes { get; set; }

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


        //自动工位
        public DbSet<Proc_GluingInfo> Proc_GluingInfos { get; set; }
        public DbSet<Proc_PressureInfo> Proc_PressureInfos { get; set; }
        public DbSet<Proc_AutoBoltInfo_Detail> Proc_AutoBoltInfo_Details { get; set; }
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

        public DbSet<Proc_ScrewNGResetRecord> Proc_ScrewNGResetRecords { get; set; }
        public DbSet<Base_ScrewNGResetConfig> Base_ScrewNGResetConfigs { get; set; }


    }
}

