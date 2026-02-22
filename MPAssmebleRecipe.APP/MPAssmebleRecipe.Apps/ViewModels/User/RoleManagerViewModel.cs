using Prism.Commands;
using Prism.Mvvm;
using Prism.Services;
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
using System.Data;
using System.Linq;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class RoleManagerViewModel : BindableBase
    {
    
        private readonly IDialogService _dialogService;
        private ObservableCollection<Role> _roles;
        private ObservableCollection<RoleManage> _managedRoles;
        private Role _selectedRole;
        private RoleManage _selectedManagedRole;
        private readonly RogerTech.AuthService.AuthService _authService;

        public ObservableCollection<Role> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }

        public ObservableCollection<RoleManage> ManagedRoles
        {
            get => _managedRoles;
            set => SetProperty(ref _managedRoles, value);
        }

        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                if (SetProperty(ref _selectedRole, value))
                {
                    LoadManagedRoles();
                    AddRoleManageCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RoleManage SelectedManagedRole
        {
            get => _selectedManagedRole;
            set => SetProperty(ref _selectedManagedRole, value);
        }

        public DelegateCommand AddRoleManageCommand { get; }
        public DelegateCommand DeleteRoleManageCommand { get; }

        public RoleManagerViewModel(IDialogService dialogService, RogerTech.AuthService.AuthService authService)
        {
            _dialogService = dialogService;
            Roles = new ObservableCollection<Role>();
            ManagedRoles = new ObservableCollection<RoleManage>();
            _authService = authService;

            AddRoleManageCommand = new DelegateCommand(ShowAddRoleManagerDialog, CanShowAddRoleManagerDialog);
            DeleteRoleManageCommand = new DelegateCommand(DeleteRoleManage, CanDeleteRoleManage)
                .ObservesProperty(() => SelectedManagedRole);

            LoadRoles();
        }

        private bool CanShowAddRoleManagerDialog()
        {
            return SelectedRole != null;
        }

        private void ShowAddRoleManagerDialog()
        {
            var parameters = new DialogParameters
            {
                { "currentRole", SelectedRole },
                { "allRoles", Roles.ToList() },
                { "managedRoleIds", ManagedRoles.Select(m => m.ManagedRoleId).ToList() }
            };

            _dialogService.ShowDialog("AddRoleManagerDialog", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    var selectedRoleIds = result.Parameters.GetValue<List<int>>("selectedRoleIds");
                    UpdateRoleManages(selectedRoleIds);
                }
                LoadRoles();
            });
        }

        private void UpdateRoleManages(List<int> selectedRoleIds)
        {
            // 删除不再被管理的角色
            var rolesToRemove = ManagedRoles
                .Where(m => !selectedRoleIds.Contains(m.ManagedRoleId))
                .ToList();

            foreach (var roleManage in rolesToRemove)
            {
                // TODO: 调用服务删除管理关系
                ManagedRoles.Remove(roleManage);
                DbContext.GetInstance().Deleteable<RoleManage>().Where(x => x.ManagerRoleId == SelectedRole.Id && x.ManagedRoleId == roleManage.ManagedRoleId).ExecuteCommand();
                OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"删除角色关系：[RoleManage:{Newtonsoft.Json.JsonConvert.SerializeObject(roleManage)}]");
            }

            // 添加新的管理角色
            var existingRoleIds = ManagedRoles.Select(m => m.ManagedRoleId);
            var newRoleIds = selectedRoleIds.Except(existingRoleIds);

            foreach (var roleId in newRoleIds)
            {
                var managedRole = Roles.First(r => r.Id == roleId);
                var roleManage = new RoleManage
                {
                    ManagerRoleId = SelectedRole.Id,
                    ManagedRoleId = roleId,
                    ManagerRole = managedRole,
                    CreatedTime = DateTime.Now
                };
                var item = DbContext.GetInstance().Queryable<RoleManage>().Where(x => x.ManagerRoleId == SelectedRole.Id && x.ManagedRoleId == roleManage.ManagedRoleId).First();
                if (item != null)
                {
                    DbContext.GetInstance().Updateable<RoleManage>(managedRole).Where(x => x.ManagerRoleId == SelectedRole.Id && x.ManagedRoleId == roleManage.ManagedRoleId).ExecuteCommand();
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Update, $"修改角色关系：[RoleManage:{Newtonsoft.Json.JsonConvert.SerializeObject(managedRole)}]");
                }
                else
                {
                    DbContext.GetInstance().Insertable<RoleManage>(roleManage).ExecuteCommand();
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Add, $"新增角色关系：[RoleManage:{Newtonsoft.Json.JsonConvert.SerializeObject(managedRole)}]");
                }
                // TODO: 调用服务添加管理关系
                ManagedRoles.Add(roleManage);
            }
            LoadManagedRoles();
        }

        private void LoadRoles()
        {
            // TODO: 从数据库加载角色列表
            var roleitems = _authService.GetAllRoles();
            Roles.Clear();
            foreach (var item in roleitems)
            {
                Roles.Add(item);

            }
        }

        private void LoadManagedRoles()
        {
            if (SelectedRole == null)
            {
                ManagedRoles.Clear();
                return;
            }

            // TODO: 根据选中的角色加载其管理的角色列表
            ManagedRoles.Clear();

            // 加载数据...
            var managedItems = DbContext.GetInstance().Queryable<RoleManage>().
               Where(x => x.ManagerRoleId == SelectedRole.Id).ToList();

            foreach (var item in managedItems)
            {
                item.ManagedRole = DbContext.GetInstance().Queryable<Role>().Where(x => x.Id == item.ManagedRoleId).First();
                ManagedRoles.Add(item);
            }

        }

        private void DeleteRoleManage()
        {
            if (SelectedManagedRole == null) return;

            var parameters = new DialogParameters
            {
                { "title", "删除确认" },
                { "message", $"确定要删除角色 \"{SelectedRole.RoleName}\" 对 \"{SelectedManagedRole.ManagedRole.RoleName}\" 的管理关系吗？" }
            };

            _dialogService.ShowDialog("ConfirmDialog", parameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    // TODO: 删除管理关系
                    LoadManagedRoles();
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Delet, $"删除角色关系：[ManagerRoleId:{SelectedRole.Id}],[RoleName:{SelectedRole.RoleName}]");
                }
            });
        }

        private bool CanDeleteRoleManage()
        {
            return SelectedManagedRole != null;
        }
    }
}