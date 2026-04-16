using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using RogerTech.Common.AuthService.Services;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RogerTech.Common.Common;
using Microsoft.Win32;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class UserViewModel : BindableBase, IRegionMemberLifetime
    {
        private readonly IDialogService _dialogService;
        private ObservableCollection<User> _users;
        private User _selectedUser;
        private readonly RogerTech.AuthService.AuthService _authService;
        private readonly IEventAggregator eventAggregator;

        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public ICommand AddUserCommand { get; }
        public DelegateCommand<User> DeleteUserCommand { get; }
        public DelegateCommand ExportUserCommand { get; }
        public DelegateCommand ImportUserCommand { get; }
        public bool KeepAlive => false;

        public UserViewModel(IDialogService dialogService, RogerTech.AuthService.AuthService authService)
        {
            _dialogService = dialogService;
            _authService = authService;
            Users = new ObservableCollection<User>();

            AddUserCommand = new DelegateCommand(AddUser);
            DeleteUserCommand = new DelegateCommand<User>(DeleteUser, CanDeleteUser).ObservesProperty(() => SelectedUser);
            ExportUserCommand = new DelegateCommand(ExecuteExport);
            ImportUserCommand = new DelegateCommand(ExecuteImport);
            LoadUsers();

        }
        private void ExecuteExport()
        {
            try
            {
                if (Users == null || Users.Count == 0) return;

                var saveFileDialog = new SaveFileDialog
                {
                    DefaultExt = ".xls",
                    Filter = "Excel文件 (*.xls)|*.xls",
                    FileName = $"用户信息_{DateTime.Now:yyyyMMddHHmmssfff}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var tempUserList = Users.Select(s => new { s.UserName, s.EmployeeId, s.RoleName, s.IsActive, s.CreatedTime }).ToList();

                    ArrayList tempColoumns;
                    var dataTable = ListToDataTableHelper.ConvertListToDataTable(tempUserList, out tempColoumns);
                    if (dataTable == null || dataTable.Rows == null || dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show($"导出失败: 转换数据失败", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    NPOIHelper.ExportExcel(dataTable, tempColoumns, "用户信息", saveFileDialog.FileName);

                    MessageBox.Show($"导出成功！文件已保存到：{saveFileDialog.FileName}",
                        "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteImport()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel文件 (*.xls)|*.xls|所有文件 (*.*)|*.*"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // 读取Excel文件
                    var dt = NPOIHelper.ExcelToDataTable(openFileDialog.FileName, true);
                    if (dt == null || dt.Rows.Count <= 0)
                    {
                        MessageBox.Show("导入文件解析异常或内容为空，请重新选择！", "错误",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    //实现导入功能具体代码
                    List<User> lstUsers = new List<User>();
                    foreach (DataRow row in dt.Rows)
                    {
                        User user = new User();
                        user.UserName = row[0].ToString();
                        user.EmployeeId = row[1].ToString();
                        user.RoleName = row[2].ToString();
                        user.IsActive = ConvertHelper.ParseBool(row[3]);
                        user.CreatedTime = ConvertHelper.GetDateTime(row[4]);
                        lstUsers.Add(user);
                    }

                    DbContext.GetInstance().UseTran(() =>
                    {
                        foreach (User item in lstUsers)
                        {
                            var user = DbContext.GetInstance().Queryable<User>().Where(p => p.EmployeeId == item.EmployeeId).ToList().FirstOrDefault();
                            if (user != null)
                            {
                                user.UserName = item.UserName;
                                user.EmployeeId = item.EmployeeId;
                                user.RoleName = item.RoleName;
                                user.IsActive = item.IsActive;
                                user.CreatedTime = item.CreatedTime;
                                DbContext.GetInstance().Updateable(user).ExecuteCommand();
                            }
                            else
                            {
                                item.PasswordHash = "888888";
                                DbContext.UserDb.Insert(item);
                            }
                        }
                    });

                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导入失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadUsers()
        {
            // TODO: 从数据库加载用户列表
            var users = _authService.GetAllUsers();
            Users = new ObservableCollection<User>();
            foreach (var item in users)
            {
                if (item.UserName!= "SystemAdmin" || item.RoleName == "SuperAdmin")
                {
                    Users.Add(item);
                }
            }
        }

        private void AddUser()
        {
            _dialogService.ShowDialog("AddUserDialog", null, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var newUser = result.Parameters.GetValue<User>("user");
                    var roleId = result.Parameters.GetValue<int?>("roleId");

                    if (newUser != null)
                    {
                        string message;
                        if (_authService.AddUser(newUser, out message))
                        {
                            if (roleId.HasValue)
                            {
                                _authService.AssignUserRole(newUser.Id, roleId.Value);
                                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Add, $"用户添加用户，新增用户名称[{newUser.UserName}],卡号[{newUser.EmployeeId}]");

                            }
                            LoadUsers(); // 重新加载用户列表以获取完整信息
                        }
                        else
                        {
                            // TODO: 显示错误消息
                        }
                    }
                }
            });
        }

        private void DeleteUser(User user)
        {
            if (user == null) return;

            var parameters = new DialogParameters
            {
                { "title", "删除确认" },
                { "message", $"确定要删除用户 \"{user.UserName}\" 吗？" }
            };

            _dialogService.ShowDialog("ConfirmDialog", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    // TODO: 从数据库删除用户
                    Users.Remove(user);
                    _authService.DeleteUser(user.Id);
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"用户删除用户，删除用户名称[{user.UserName}],卡号[{user.EmployeeId}]");
                    if (SelectedUser == user)
                    {
                        SelectedUser = null;
                    }
                }
            });
        }

        private bool CanDeleteUser(User user)
        {
            return user != null;
        }
    }
}
