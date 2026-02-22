using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using RogerTech.Common.AuthService.Model;
using RogerTech.Common;
using SqlSugar;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class UcOperationViewModel : BindableBase
    {

        public UcOperationViewModel()
        {
            Logs = new ObservableCollection<UserOperationLog>();
            LoadData();
        }


        private int _currentPage = 1;
        private int _pageSize = 20;
        private int _totalPages;
        public ObservableCollection<UserOperationLog> Logs { get; set; }

        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        private async void LoadData()
        {
            var totalCount = await DbContext.GetInstance().Queryable<UserOperationLog>().CountAsync();
            TotalPages = (int)Math.Ceiling((double)totalCount / _pageSize);

            var pageData = await DbContext.GetInstance().Queryable<UserOperationLog>()
                .OrderBy(x => x.Id, OrderByType.Desc)
                .ToPageListAsync(CurrentPage, _pageSize);

            Logs.Clear();
            foreach (var item in pageData)
                Logs.Add(item);
        }

        private DelegateCommand _firstPageConmmand;
        public DelegateCommand FirstPageCommand
        {
            get
            {
                if (_firstPageConmmand == null)
                {
                    _firstPageConmmand = new DelegateCommand(() =>
                    {
                        if (CurrentPage != 1)
                        {
                            CurrentPage = 1;
                            LoadData();
                        }
                    });
                }
                return _firstPageConmmand;
            }
        }

        private DelegateCommand _prevPageCommand;
        public DelegateCommand PrevPageCommand
        {
            get
            {
                if (_prevPageCommand == null)
                {
                    _firstPageConmmand = new DelegateCommand(() =>
                    {
                        if (CurrentPage > 1)
                        {
                            CurrentPage--;
                            LoadData();
                        }
                    });
                }
                return _prevPageCommand;
            }
        }

        private DelegateCommand _nextPageCommand;
        public DelegateCommand NextPageCommand
        {
            get
            {
                if (_nextPageCommand == null)
                {
                    _nextPageCommand = new DelegateCommand(() =>
                    {
                        if (CurrentPage < TotalPages)
                        {
                            CurrentPage++;
                            LoadData();
                        }
                    });
                }
                return _nextPageCommand;
            }
        }

        private DelegateCommand _previousPageCommand;
        public DelegateCommand LastPageCommand
        {
            get
            {
                if (_previousPageCommand == null)
                {
                    _previousPageCommand = new DelegateCommand(() =>
                    {
                        if (CurrentPage != TotalPages)
                        {
                            CurrentPage = TotalPages;
                            LoadData();
                        }
                    });
                }
               return _previousPageCommand;
            }
        }

    }
}
