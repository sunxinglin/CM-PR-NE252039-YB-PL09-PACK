using CatlMesBase;
using MPAssmebleRecipe.Models.Entities.Issues;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using RogerTech.BussnessCore;
using RogerTech.Common;
using RogerTech.Common.AuthService.Model;
using RogerTech.Common.AuthService.Services;
using RogerTech.Common.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Xml.Linq;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class UploadDataViewModel : BindableBase, IRegionMemberLifetime
    {
        protected string StationName = ConfigurationManager.AppSettings["StationName"];

        private readonly IDialogService _dialogService;
        private BussnessUtility business = BussnessUtility.GetInstanse();
        private string Dbname= "UploadData";
        #region 属性
        private string _SFC;
        public string SFC
        {
            get { return _SFC; }
            set { SetProperty(ref _SFC, value); }
        }

        private DateTime? _startDate = DateTime.Now.Date;
        public DateTime? StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }

        private TimeSpan? _startTime;
        public TimeSpan? StartTime
        {
            get { return _startTime; }
            set { SetProperty(ref _startTime, value); }
        }

        private DateTime? _endDate = DateTime.Now.Date.AddDays(1);
        public DateTime? EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }

        private TimeSpan? _endTime;
        public TimeSpan? EndTime
        {
            get { return _endTime; }
            set { SetProperty(ref _endTime, value); }
        }

        private List<UploadData> _Datas;
        public List<UploadData> Datas
        {
            get { return _Datas; }
            set { SetProperty(ref _Datas, value); }
        }

        private List<UploadData> _SelectDatas;
        public List<UploadData> SelectDatas
        {
            get { return _SelectDatas; }
            set { SetProperty(ref _SelectDatas, value); }
        }

        private int _pageSize = 30;

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        private int _totalPages;
        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }
        #endregion

        #region 命令


        public ICommand LocalRefreshCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand UploadCommand { get; private set; }

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
                            LoadDatas();
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
                    _prevPageCommand = new DelegateCommand(() =>
                    {
                        if (CurrentPage > 1)
                        {
                            CurrentPage--;
                            LoadDatas();
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
                            LoadDatas();
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
                            LoadDatas();
                        }
                    });
                }
                return _previousPageCommand;
            }
        }

        public bool KeepAlive => true;

        #endregion

        public UploadDataViewModel(IDialogService dialogService)
        {
            InitializeCommands();
            LoadDatas();
            InitializeDefaultValues();
            _dialogService = dialogService;
            DbContext.ProgressInfoChange += new Action(() =>
            {
                LoadDatas();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeCommands()
        {
            RefreshCommand = new DelegateCommand(RefreshLogs);
            UploadCommand = new DelegateCommand(UploadDatas);   
            LocalRefreshCommand = new DelegateCommand(LocalRefreshLogs);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeDefaultValues()
        {
            StartDate = DateTime.Today;
            StartTime = new TimeSpan(0, 0, 0);
            EndDate = DateTime.Today;
            EndTime = new TimeSpan(23, 59, 59);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadDatas()
        {
            DateTime startDateTime = StartDate.GetValueOrDefault(DateTime.Today);
            if (StartTime.HasValue)
            {
                startDateTime = startDateTime.Date + StartTime.Value;
            }
            else
            {
                startDateTime = startDateTime.Date + new TimeSpan(0, 0, 0);
            }

            DateTime endDateTime = EndDate.GetValueOrDefault(DateTime.Today);
            if (EndTime.HasValue)
            {
                endDateTime = endDateTime.Date + EndTime.Value;
            }
            else
            {
                endDateTime = endDateTime.Date + new TimeSpan(23, 59, 59);
            }

            int totalCount = 0;
            List<UploadData> pageData = null;
            if (!string.IsNullOrEmpty(SFC))
            {
                totalCount = DbContext.GetInstance().Queryable<UploadData>().AS(Dbname).Where(p => p.StationName == StationName && p.IsReupload == false && p.Time >= startDateTime && p.Time < endDateTime && p.SFC.Contains(SFC)).Count();
                TotalPages = (int)Math.Ceiling((double)totalCount / _pageSize);
                pageData = DbContext.GetInstance().Queryable<UploadData>().AS(Dbname).Where(p => p.StationName == StationName && p.IsReupload == false && p.Time >= startDateTime && p.Time < endDateTime && p.SFC.Contains(SFC)
                ).OrderBy(x => x.SFC, OrderByType.Desc).OrderBy(s => s.InterfaceName).OrderBy(o => o.UploadDataName).ToPageList(CurrentPage, _pageSize);
            }
            else
            {
                totalCount = DbContext.GetInstance().Queryable<UploadData>().AS(Dbname).Where(p => p.StationName == StationName && p.IsReupload == false && p.Time >= startDateTime && p.Time < endDateTime).Count();
                TotalPages = (int)Math.Ceiling((double)totalCount / _pageSize);
                pageData = DbContext.GetInstance().Queryable<UploadData>().AS(Dbname).Where(p => p.StationName == StationName && p.IsReupload == false && p.Time >= startDateTime && p.Time < endDateTime
                ).OrderBy(x => x.SFC, OrderByType.Desc).OrderBy(s => s.InterfaceName).OrderBy(o => o.UploadDataName).ToPageList(CurrentPage, _pageSize);
            }
          
            Datas = pageData;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshLogs()
        {
            CurrentPage = 1;
            Dbname = "UploadData";
            LoadDatas();
        }
        private void LocalRefreshLogs()
        {
            CurrentPage = 1;
            Dbname = "localdata";
            LoadDatas();
        }
        /// <summary>
        /// 
        /// </summary>
        private void UploadDatas()
        {

            //接口实现调用
            if (SelectDatas != null && SelectDatas.Count > 0 && Dbname == "UploadData")
            {
                KeyValuePair<string, string> key = business.MesInvokeMesCollectData(SelectDatas);

                if (key.Key == "0")
                {

                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Update, $"手动上传MES数据成功，MES数据：[{Newtonsoft.Json.JsonConvert.SerializeObject(SelectDatas)}]");
                    MessageBox.Show($"数据上传成功");
                }
                else
                {
                    OperationService.OperationRecord(RogerTech.Common.AuthService.Model.Operation.Update, $"手动上传MES数据失败，报错代码：{key.Key}，报错内容：{key.Value}，MES数据：[{Newtonsoft.Json.JsonConvert.SerializeObject(SelectDatas)}]");
                    MessageBox.Show($"数据上传失败，报错代码：{key}，报错内容：{key.Value}");
                }
            }
            else
            {
                MessageBox.Show($"请选择要上传的数据（本地数据无法上传）");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        public void ShowCodeDetail(int code)
        {
            var parameters = new DialogParameters { { "dataEntry", code } };
            _dialogService.ShowDialog("UploadDataView", parameters, _ => { });
        }
    }
}
