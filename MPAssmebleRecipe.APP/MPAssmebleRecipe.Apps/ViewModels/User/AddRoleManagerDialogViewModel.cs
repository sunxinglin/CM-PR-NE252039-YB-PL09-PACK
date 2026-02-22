using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Linq;
using RogerTech.AuthService.Models;
using System;
using System.Collections.Generic;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class AddRoleManagerDialogViewModel : BindableBase, IDialogAware
    {
        private Role _currentRole;
        private bool _isAllSelected;
        private ObservableCollection<RoleSelectionViewModel> _availableRoles;

        public string Title => "添加角色管理关系";

        public Role CurrentRole
        {
            get => _currentRole;
            set => SetProperty(ref _currentRole, value);
        }

        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (SetProperty(ref _isAllSelected, value))
                {
                    foreach (var role in AvailableRoles)
                    {
                        role.IsSelected = value;
                    }
                }
            }
        }

        public ObservableCollection<RoleSelectionViewModel> AvailableRoles
        {
            get => _availableRoles;
            set => SetProperty(ref _availableRoles, value);
        }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public event Action<IDialogResult> RequestClose;

        public AddRoleManagerDialogViewModel()
        {
            AvailableRoles = new ObservableCollection<RoleSelectionViewModel>();
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel)));
        }

        private void Save()
        {
            var selectedRoleIds = AvailableRoles
                .Where(r => r.IsSelected)
                .Select(r => r.Id)
                .ToList();

            var parameters = new DialogParameters
            {
                { "selectedRoleIds", selectedRoleIds }
            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            CurrentRole = parameters.GetValue<Role>("currentRole");
            var allRoles = parameters.GetValue<List<Role>>("allRoles");
            var managedRoleIds = parameters.GetValue<List<int>>("managedRoleIds");

            AvailableRoles.Clear();
            foreach (var role in allRoles)
            {
                AvailableRoles.Add(new RoleSelectionViewModel
                {
                    Id = role.Id,
                    RoleName = role.RoleName,
                    Description = role.Description,
                    CreatedTime = role.CreatedTime,
                    IsSelected = managedRoleIds.Contains(role.Id)
                });
            }

            UpdateAllSelectedState();
        }

        private void UpdateAllSelectedState()
        {
            IsAllSelected = AvailableRoles.Any() && AvailableRoles.All(r => r.IsSelected);
        }

        public void OnDialogClosed()
        {
        }
    }

    public class RoleSelectionViewModel : BindableBase
    {
        private bool _isSelected;

        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedTime { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }
} 