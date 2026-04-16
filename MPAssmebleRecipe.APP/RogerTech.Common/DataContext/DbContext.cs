using log4net;
using RogerTech.AuthService.Models;
using RogerTech.Common.AuthService.Model;
using RogerTech.Common.Models;
using SqlSugar;
using System;
using System.Configuration;

namespace RogerTech.Common
{
    public class DbContext
    {
        private static SqlSugarScope _instance;

        private static readonly ILog _logger = LogManager.GetLogger(nameof(DbContext));

        private static readonly Object locker = new object();


        public static SqlSugarScope GetInstance()
        {
            try
            {
                if (_instance == null)
                {
                    string connStr = ConfigurationManager.AppSettings["SqlConnectStr"];
                    if (string.IsNullOrEmpty(connStr))
                    {
                        throw new ConfigurationErrorsException("未找到数据库连接字符串配置");
                    }

                    _instance = new SqlSugarScope(new ConnectionConfig
                    {
                        DbType = DbType.PostgreSQL,
                        ConnectionString = connStr,
                        IsAutoCloseConnection = true,
                        InitKeyType = InitKeyType.Attribute, //从实体特性中读取主键自增配置
                        MoreSettings = new ConnMoreSettings
                        {
                            IsAutoRemoveDataCache = true //自动清理缓存
                        },
                        AopEvents = new AopEvents
                        {
                            OnLogExecuting = (sql, param) =>
                            {
                                // _logger.Debug($"SQL执行: {sql}");
                            },
                            OnLogExecuted = (sql, param) =>
                            {
                                //_logger.Debug($"SQL完成: {sql}");
                            },
                            OnError = (ex) =>
                            {
                                //_logger.Error($"SQL错误: {ex.Message}");
                            }
                        }
                    });

                    InitDatabase();
                }

                return _instance;
            }
            catch (Exception ex)
            {
                // _logger.Error(ex, new Exception("初始化数据库连接失败"));
                throw new Exception("初始化数据库连接失败", ex);
            }
        }
        private static void InitDatabase()
        {
            try
            {
                // _logger.Info("开始初始化数据库和表");

                // 创建数据库
                _instance.DbMaintenance.CreateDatabase();

                // 初始化表
                _instance.CodeFirst.InitTables<User>();
                _instance.CodeFirst.InitTables<Role>();
                _instance.CodeFirst.InitTables<Menu>();
                _instance.CodeFirst.InitTables<RoleMenu>();
                _instance.CodeFirst.InitTables<UserRole>();
                _instance.CodeFirst.InitTables<Workstation>();
                _instance.CodeFirst.InitTables<UserWorkstation>();
                _instance.CodeFirst.InitTables<RoleManage>();
                _instance.CodeFirst.InitTables<UserOperationLog>();
                _instance.CodeFirst.InitTables<LogModel>();

                //  _instance.CodeFirst.InitTables<PackTemplate>();
                // _instance.CodeFirst.InitTables<RecipeItem>();
                //   _instance.CodeFirst.InitTables<LocalData>();
                _instance.CodeFirst.InitTables<UploadData>();
                _instance.CodeFirst.As<UploadData>("LocalData").InitTables<UploadData>();

                //新增

                _instance.CodeFirst.InitTables<MHRUser>();
                //  _instance.CodeFirst.InitTables<Recipe_Temp>();
                _instance.CodeFirst.InitTables<Production_Recipe>();
                _instance.CodeFirst.InitTables<TemplateDto>();
                _instance.CodeFirst.InitTables<Template_Cell>();
                _instance.CodeFirst.InitTables<Template_Module>();
                _instance.CodeFirst.InitTables<Template_Block>();
                _instance.CodeFirst.InitTables<Template_Pack>();

				//// 配方记录表
				//_instance.MappingTables.Add("RecipeItem", "recipe_record");
				//_instance.CodeFirst.SetStringDefaultLength(200).InitTables<RecipeItem>();
				//_instance.MappingTables.Clear();

				//// 配方临时表
				//_instance.MappingTables.Add("RecipeItem", "recipe_temp");
				//_instance.CodeFirst.SetStringDefaultLength(200).InitTables<RecipeItem>();
				//_instance.MappingTables.Clear();


				CreateSeedUsers(_instance);
                
            }
            catch (Exception ex)
            {
                //_logger.Error(ex, "初始化数据库表失败");
                throw new Exception("初始化数据库表失败", ex);
            }
		}

        #region 开发环境初始化超管用户

        private static void CreateSeedUsers(SqlSugarScope db)
        {
            if (ConfigurationManager.AppSettings["Environment"] != "dev")
            {
                return;
            }

            var superRole = db.Queryable<Role>().Where(r => r.RoleName == "SuperAdmin").First();
            if (superRole == null)
            {
                superRole = new Role
                {
                    RoleName = "SuperAdmin",
                    Description = "测试环境最高权限角色",
                };
                db.Insertable(superRole).ExecuteCommand();
            }
            
            var user = db.Queryable<User>().Where(u => u.EmployeeId == "000000").First();
            if (user == null)
            {
                user = new User
                {
                    UserName = "SystemAdmin",
                    EmployeeId = "000000",
                    PasswordHash = "password",
                    RoleName = superRole.RoleName
                };
                db.Insertable(user).ExecuteCommand();
            }
            
            user = db.Queryable<User>().Where(u => u.EmployeeId == "000001").First();
            if (user == null)
            {
                user = new User
                {
                    UserName = "SuperAdmin01",
                    EmployeeId = "000001",
                    PasswordHash = "123",
                    RoleName = superRole.RoleName
                };
                db.Insertable(user).ExecuteCommand();
            }

            user = db.Queryable<User>().Where(u => u.EmployeeId == "000002").First();
            if (user == null)
            {
                user = new User
                {
                    UserName = "SuperAdmin02",
                    EmployeeId = "000002",
                    PasswordHash = "123456",
                    RoleName = superRole.RoleName
                };
                db.Insertable(user).ExecuteCommand();
            }

        }

        #endregion

        
        public static Action ProgressInfoChange;

        public static Action<string> ProgressErrorChange;

        public static void Info(string productID, string message, int code, string  groupName)
        {
            lock (locker)
            {
                LogModel logModel = new LogModel
                {
                    InterfaceName = groupName,
                    LogType = "INFO",
                    RecordTime = DateTime.Now,
                    Result = code.ToString(),
                    ProductID = productID,
                    Message = message
                };
                try
                {
                    // ToOptimize:如果没人调用过GetInstance()方法，会抛出空引用异常
                    _instance.Insertable(logModel).ExecuteCommand();
                    ProgressInfoChange?.Invoke();
                }
                catch (Exception ex)
                {
                    Error(productID, message, 9999, groupName);
                }
            }

        }

        public static void Error(string productID, string message, int code, string groupName)
        {

            lock (locker)
            {
                try
                {
                    LogModel logModel = new LogModel
                    {
                        InterfaceName = groupName,
                        LogType = "ERROR",
                        RecordTime = DateTime.Now,
                        ProductID = productID,
                        Result = code.ToString(),
                        Message = message,
                    };
                    // ToOptimize:如果没人调用过GetInstance()方法，会抛出空引用异常
                    _instance.Insertable(logModel).ExecuteCommand();
                    ProgressErrorChange?.Invoke("");
                }
                catch (Exception ex)
                {
                    ProgressErrorChange?.Invoke(ex.Message);
                }
            }
        }
        #region 仓储
        public static SimpleClient<User> UserDb => new SimpleClient<User>(_instance);
        public static SimpleClient<Role> RoleDb => new SimpleClient<Role>(_instance);
        public static SimpleClient<Menu> MenuDb => new SimpleClient<Menu>(_instance);
        public static SimpleClient<RoleMenu> RoleMenuDb => new SimpleClient<RoleMenu>(_instance);
        public static SimpleClient<UserRole> UserRoleDb => new SimpleClient<UserRole>(_instance);
        public static SimpleClient<Workstation> WorkstationDb => new SimpleClient<Workstation>(_instance);
        public static SimpleClient<UserWorkstation> UserWorkstationDb => new SimpleClient<UserWorkstation>(_instance);
        #endregion

    }
}
