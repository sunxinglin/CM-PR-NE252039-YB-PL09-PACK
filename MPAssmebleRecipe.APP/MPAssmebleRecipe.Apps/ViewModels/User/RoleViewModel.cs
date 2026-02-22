using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Regions;
using Prism.Services.Dialogs;
using RogerTech.AuthService;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using RogerTech.Common.AuthService.Model;
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
    public class RoleViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;
        private Role _selectedRole;
        private ObservableCollection<Menu> _menus;
        private readonly AuthService _authService;

        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                {
                    LoadRoleMenus();
                }
            }
        }
        public ObservableCollection<Role> Roles { get; private set; }

        public ObservableCollection<Menu> Menus
        {
            get => _menus;
            set => SetProperty(ref _menus, value);
        }

        public ICommand AddRoleCommand { get; private set; }
        public DelegateCommand<Role> DeleteRoleCommand { get; private set; }
        public DelegateCommand<Role> EditMenusCommand { get; private set; }

        public RoleViewModel(IDialogService dialogService, AuthService authService)
        {
            _dialogService = dialogService;
            _authService = authService;

            Roles = new ObservableCollection<Role>();
            Menus = new ObservableCollection<Menu>();

            LoadRoles();

            // 初始化命令
            AddRoleCommand = new DelegateCommand(ShowAddRoleDialog);
            DeleteRoleCommand = new DelegateCommand<Role>(DeleteRole, CanDeleteRole)
                .ObservesProperty(() => SelectedRole);
            EditMenusCommand = new DelegateCommand<Role>(EditMenus, CanEditMenus)
                .ObservesProperty(() => SelectedRole);
        }

        private void EditMenus(Role role)
        {
            if (role == null) return;

            var allMenus = _authService.GetAllMenus();
            var currentRoleMenus = _authService.GetRoleMenus(role.Id).ToList();
            var selectedMenuIds = currentRoleMenus.Select(x => x.Id).ToList();
            //var selectedMenuIds = new List<int>();
            var parameters = new DialogParameters
            {
                { "allMenus", allMenus },
                { "selectedMenuIds", selectedMenuIds }
            };

            _dialogService.ShowDialog("MenuSelectionDialog", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var newSelectedMenuIds = result.Parameters.GetValue<List<int>>("selectedMenuIds");

                    // 删除已移除的菜单
                    var menuIdsToRemove = selectedMenuIds.Except(newSelectedMenuIds);
                    foreach (var menuId in menuIdsToRemove)
                    {
                        //_authService.RemoveRoleMenu(role.Id, menuId);
                        DbContext.GetInstance().Deleteable<RoleMenu>().Where(x => x.RoleId == SelectedRole.Id && x.MenuId == menuId).ExecuteCommand();
                        OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"删除权限，权限信息：[RoleId:{SelectedRole.Id}],[MenuId:{menuId}]");
                    }

                    // 添加新选择的菜单
                    var menuIdsToAdd = newSelectedMenuIds.Except(selectedMenuIds);
                    foreach (var menuId in menuIdsToAdd)
                    {
                        RoleMenu rolemenu = new RoleMenu()
                        {
                            RoleId = role.Id,
                            MenuId = menuId
                        };
                        var item = DbContext.GetInstance().Queryable<RoleMenu>().Where(x => x.RoleId == rolemenu.RoleId && x.MenuId == rolemenu.MenuId).First();
                        if (item == null)
                        {
                            DbContext.GetInstance().Insertable<RoleMenu>(rolemenu).ExecuteCommand();
                            OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Add, $"新增权限，权限信息：[RoleId:{rolemenu.RoleId}],[MenuId:{rolemenu.MenuId}]");
                        }
                    }

                    // 刷新菜单列表
                    LoadRoleMenus();
                }
            });
        }

        private bool CanEditMenus(Role role)
        {
            return role != null;
        }

        private void LoadRoleMenus()
        {
            if (SelectedRole != null)
            {
                var roleMenus = _authService.GetRoleMenus(SelectedRole.Id);
                Menus.Clear();
                foreach (var item in roleMenus)
                {
                    Menus.Add(item);
                }
            }
            else
            {
                Menus.Clear();
            }
        }

        private void LoadRoles()
        {
            var roles = _authService.GetAllRoles();
            Roles = new ObservableCollection<Role>(roles);
        }

        private bool CanDeleteRole(Role role)
        {
            return role != null;
        }

        private void DeleteRole(Role role)
        {
            if (role != null)
            {
                var parameters = new DialogParameters
                {
                    { "title", "删除确认" },
                    { "message", $"确定要删除角色 \"{role.RoleName}\" 吗？" }
                };

                _dialogService.ShowDialog("ConfirmDialog", parameters, result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        _authService.DeleteRole(role.Id);
                        Roles.Remove(role);
                        if (SelectedRole == role)
                        {
                            SelectedRole = null;
                        }
                        OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"删除角色，角色信息：[RoleId:{role.Id}],[MenuId:{role.RoleName}]");
                    }
                });
            }
        }

        private void ShowAddRoleDialog()
        {
            _dialogService.ShowDialog("AddRoleView", null, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var role = result.Parameters.GetValue<Role>("role");
                    if (_authService.AddRole(role))
                    {
                        LoadRoles();
                    }
                }
            });
        }
    }
}
