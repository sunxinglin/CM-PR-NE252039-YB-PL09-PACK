using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Linq;
using RogerTech.AuthService.Models;
using System;
using System.Collections.Generic;
using RogerTech.AuthService.Models;
using System.Windows.Controls;
using Menu = RogerTech.AuthService.Models.Menu;

namespace UserTest.ViewModels
{
    public class MenuSelectionDialogViewModel : BindableBase, IDialogAware
    {
        private bool _isAllSelected;
        private ObservableCollection<MenuItemViewModel> _menuItems;

        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (SetProperty(ref _isAllSelected, value))
                {
                    foreach (var item in MenuItems)
                    {
                        item.IsSelected = value;
                    }
                }
            }
        }

        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get => _menuItems;
            set {
                SetProperty(ref _menuItems, value);
                UpdateAllSelectedState();
            }
        }

        public string Title => "选择菜单";

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand SaveCommand { get; }
        public DelegateCommand CancelCommand { get; }

        public MenuSelectionDialogViewModel()
        {
            MenuItems = new ObservableCollection<MenuItemViewModel>();
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var allMenus = parameters.GetValue<List<Menu>>("allMenus");
            var selectedMenuIds = parameters.GetValue<List<int>>("selectedMenuIds");

            MenuItems.Clear();
            //menuites 和传入的menus比对
            foreach (var menu in allMenus)
            {
                if(selectedMenuIds.Contains(menu.Id))
                {
                    MenuItemViewModel selectMenu = new MenuItemViewModel()
                    {
                        Id = menu.Id,
                        MainPage = menu.Page,
                        SubPage = menu.SubPage,
                        Button = menu.ElementName,
                    };
                    selectMenu.IsSelected = true;
                    MenuItems.Add(selectMenu);
                }
                else
                {
                    MenuItemViewModel selectMenu = new MenuItemViewModel() {
                        Id = menu.Id,
                        MainPage = menu.Page,
                        SubPage = menu.SubPage,
                        Button = menu.ElementName,
                    };
                    selectMenu.IsSelected = false;
                    MenuItems.Add(selectMenu);
                }
            }

            UpdateAllSelectedState();
        }

        private void UpdateAllSelectedState()
        {
            _isAllSelected = MenuItems.All(x => x.IsSelected);
            RaisePropertyChanged(nameof(IsAllSelected));
        }

        private void Save()
        {
            var selectedMenuIds = MenuItems.Where(x => x.IsSelected)
                                         .Select(x => x.Id)
                                         .ToList();

            var parameters = new DialogParameters
            {
                { "selectedMenuIds", selectedMenuIds }
            };

            RequestClose?.Invoke(new DialogResult(ButtonResult.OK, parameters));
        }

        private void Cancel()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }
    }

    public class MenuItemViewModel : BindableBase
    {
        private bool _isSelected;
        
        public int Id { get; set; }
        public string MainPage { get; set; }
        public string SubPage { get; set; }
        public string Button { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

    public class SelectMenu:Menu
    {
        public bool Selected { get; set; }
    }
} 