using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using RogerTech.AuthService.Models;
using RogerTech.Common.AuthService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class AddRoleViewModel : BindableBase, IDialogAware
    {
        private string _roleName;
        private string _description;
        private readonly IDialogService _dialogService;

        public AddRoleViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            SaveCommand = new DelegateCommand(Save, CanSave).ObservesProperty(() => RoleName);
            CancelCommand = new DelegateCommand(Cancel);
        }

        public string RoleName
        {
            get => _roleName;
            set => SetProperty(ref _roleName, value);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public string Title => "添加角色";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public List<Role> Roles { get; private set; }
        public void OnDialogOpened(IDialogParameters parameters)
        {
            //var roleStr = parameters.GetValue<string>("key");
            //var obserRoles = JsonConvert.DeserializeObject<ObservableCollection<Role>>(roleStr);
        }

        private bool CanSave()
        {
            return (!string.IsNullOrWhiteSpace(RoleName));
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(Description))
            {
                return;
            }
            var role = new Role
            {
                RoleName = RoleName?.Trim(),
                Description = Description?.Trim(),
                CreatedTime = DateTime.Now
            };

            var parameters = new DialogParameters
            {
                { "role", role }
            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }
    }
} 