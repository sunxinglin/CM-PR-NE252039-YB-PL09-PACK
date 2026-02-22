using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Windows.Controls;
using RogerTech.AuthService.Models;
using System.Collections.ObjectModel;
using RogerTech.AuthService;
using System;

namespace UserTest.ViewModels
{
    public class AddUserDialogViewModel : BindableBase, IDialogAware
    {
        private readonly AuthService _authService;
        private string _userName;
        private string _employeeId;
        private ObservableCollection<Role> _roles;
        private Role _selectedRole;

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string EmployeeId
        {
            get => _employeeId;
            set => SetProperty(ref _employeeId, value);
        }

        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }

        public string Title => "添加用户";

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<PasswordBox> SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public AddUserDialogViewModel(AuthService authService)
        {
            _authService = authService;
            SaveCommand = new DelegateCommand<PasswordBox>(Save);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
            
            LoadRoles();
        }

        private void LoadRoles()
        {
            var roleList = _authService.GetAllRoles();
            Roles = new ObservableCollection<Role>(roleList);
        }

        private void Save(PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(UserName) || 
                string.IsNullOrWhiteSpace(EmployeeId) || 
                string.IsNullOrWhiteSpace(passwordBox?.Password))
            {
                // TODO: 显示错误提示
                return;
            }
            if (SelectedRole == null) {
                return;
            }

            var user = new User
            {
                UserName = UserName,
                EmployeeId = EmployeeId,
                PasswordHash = passwordBox.Password, // 注意：实际应用中需要加密
                CreatedTime = DateTime.Now,
                RoleName = SelectedRole.RoleName,
                IsActive = true
            };

            var parameters = new DialogParameters
            {
                { "user", user },
                { "roleId", SelectedRole?.Id }
            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
} 