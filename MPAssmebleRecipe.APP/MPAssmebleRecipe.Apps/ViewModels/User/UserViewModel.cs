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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public bool KeepAlive => false;

        public UserViewModel(IDialogService dialogService, RogerTech.AuthService.AuthService authService)
        {
            _dialogService = dialogService;
            _authService = authService;
            Users = new ObservableCollection<User>();

            AddUserCommand = new DelegateCommand(AddUser);
            DeleteUserCommand = new DelegateCommand<User>(DeleteUser, CanDeleteUser).ObservesProperty(() => SelectedUser);

            LoadUsers();

        }

        private void LoadUsers()
        {
            // TODO: 从数据库加载用户列表
            var users = _authService.GetAllUsers();
            Users = new ObservableCollection<User>();
            foreach (var item in users)
            {
                if (item.UserName!= "SystemAdmin")
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
