using SqlSugar;
using RogerTech.AuthService.Models;
using System.Reflection;
using System.Configuration;
using RogerTech.Common.AuthService.Model;
using System.Threading.Tasks;
using System;
using log4net;
using System.Collections.Generic;
using RogerTech.Common.Models;
using System.Net.Sockets;
using RogerTech.Tool;
using MPAssmebleRecipe.Models.Entities.Issues;

namespace RogerTech.Common
{
    public class DbContext
    {
        private static SqlSugarScope _instance;

        ILog logger = LogManager.GetLogger("");

        public static readonly Object locker = new object();


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

                    _instance = new SqlSugarScope(new ConnectionConfig()
                    {
                        DbType = SqlSugar.DbType.PostgreSQL,
                        ConnectionString = connStr,
                        IsAutoCloseConnection = true,
                        InitKeyType = InitKeyType.Attribute, //从实体特性中读取主键自增配置
                        MoreSettings = new ConnMoreSettings()
                        {
                            IsAutoRemoveDataCache = true //自动清理缓存
                        },
                        AopEvents = new AopEvents
                        {
                            OnLogExecuting = (sql, param) =>
                            {
                                //_logger.Debug($"SQL执行: {sql}");
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
                //_logger.Error(ex, "初始化数据库连接失败");
                throw new Exception("初始化数据库连接失败", ex);
            }
        }
        private static void InitDatabase()
        {
            try
            {
                //_logger.Info("开始初始化数据库和表");

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

            }
            catch (Exception ex)
            {
                //_logger.Error(ex, "初始化数据库表失败");
                throw new Exception("初始化数据库表失败", ex);
            }
            finally
            {
                // 检查是否存在记录
                var userExists = GetInstance().Queryable<User>().Any(u => u.UserName == "SystemAdmin" && u.EmployeeId == "000000");

                if (!userExists)
                {
                    // 插入新记录
                    var newUser = new User
                    {
                        UserName = "SystemAdmin",
                        EmployeeId = "000000",
                        PasswordHash = "password",    
                        RoleName="",
                        IsActive = true,
                        CreatedTime = DateTime.Now
                    };

                    GetInstance().Insertable(newUser).ExecuteCommand();
                }
            }

        }



        
        public static Action ProgressInfoChange;

        public static Action<string> ProgressErrorChange;

        public static void Info(string proudctID, string message, int code, string  GroupName)
        {
            lock (locker)
            {
                LogModel logModel = new LogModel()
                {
                    InterfaceName = GroupName,
                    LogType = "INFO",
                    RecordTime = DateTime.Now,
                    Result = code.ToString(),
                    ProductID = proudctID,
                    Message = message
                };
                try
                {
                    _instance.Insertable<LogModel>(logModel).ExecuteCommand();
                    ProgressInfoChange?.Invoke();
                }
                catch (Exception ex)
                {
                    Error(proudctID, message, 9999, GroupName);
                }
            }

        }

        public static void Error(string proudctID, string message, int code, string GroupName)
        {

            lock (locker)
            {
                try
                {
                    LogModel logModel = new LogModel()
                    {
                        InterfaceName = GroupName,
                        LogType = "ERROR",
                        RecordTime = DateTime.Now,
                        ProductID = proudctID,
                        Result = code.ToString(),
                        Message = message,
                    };
                    _instance.Insertable<LogModel>(logModel).ExecuteCommand();
                    ProgressErrorChange?.Invoke("");
                }
                catch (Exception ex)
                {
                    ProgressErrorChange?.Invoke(ex.Message);
                }

                finally
                {

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