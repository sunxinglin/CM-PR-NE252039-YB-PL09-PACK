using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Ctp0600P.Client.Command.NoticeHelp.Enum;
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using Ctp0600P.Client.Protocols;

using FutureTech.Mvvm;

using MediatR;

using Yee.Entitys.DTOS;

namespace Ctp0600P.Client.ViewModels
{
    public class HistoryPageViewModel : ViewModelBase
    {
        public HistoryPageViewModel(IMediator mediator, IAPIHelper apiHelper)
        {
            this._mediator = mediator;
            this._apiHelper = apiHelper;

            InitSearchFilter();

            #region 查询历史
            this.SearchHisData = new AsyncRelayCommand<object>(
                async s =>
                {
                    await SearchAsync();
                },
                o => true);
            #endregion
        }

        public void InitSearchFilter()
        {
            if (DateTime.Now.Hour > App._StepStationSetting.DayEndTime)
            {
                Date_Begin = DateTime.Now.Date;
                Time_Begin = DateTime.Parse($"{App._StepStationSetting.DayEndTime}:00");

                Date_End = DateTime.Now.AddDays(1).Date;
                Time_End = DateTime.Parse($"{App._StepStationSetting.DayStartTime}:00");
            }
            else if (DateTime.Now.Hour < App._StepStationSetting.DayStartTime)
            {
                Date_Begin = DateTime.Now.AddDays(-1).Date;
                Time_Begin = DateTime.Parse($"{App._StepStationSetting.DayEndTime}:00");

                Date_End = DateTime.Now.Date;
                Time_End = DateTime.Parse($"{App._StepStationSetting.DayStartTime}:00");
            }
            else
            {
                Date_Begin = DateTime.Now.Date;
                Time_Begin = DateTime.Parse($"{App._StepStationSetting.DayStartTime}:00");

                Date_End = DateTime.Now.Date;
                Time_End = DateTime.Parse($"{App._StepStationSetting.DayEndTime}:00");
            }
        }

        public async Task SearchAsync()
        {
            try
            {
                var dateBegin = Date_Begin.ToString("yyyy-MM-dd ");
                var timeBegin = Time_Begin.ToString("HH:mm:ss");
                var searchBeginTime = DateTime.Parse(dateBegin + timeBegin);

                var dateEnd = Date_End.ToString("yyyy-MM-dd ");
                var timeEnd = Time_End.ToString("HH:mm:ss");
                var searchEndTime = DateTime.Parse(dateEnd + timeEnd);

                if (searchBeginTime > searchEndTime)
                {
                    var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"结束时间必须大于开始时间" };
                    await this._mediator.Publish(notice);
                    return;
                }

                PackTaskMainDataDTO mainData = await _apiHelper.LoadStepPackTaskMainData(searchBeginTime, searchEndTime, App._StepStationSetting.StationCode);
                if (mainData == null || mainData.Items == null || mainData.Items.Count == 0)
                {
                    PCount = 0;
                    ProductList = new ObservableCollection<PackTaskRecordDataDTO>();
                }
                else
                {
                    PCount = mainData.TotalCount;
                    ProductList = new ObservableCollection<PackTaskRecordDataDTO>(mainData.Items);
                }
            }
            catch (Exception ex)
            {
                var notice = new MessageNotice() { messageType = MessageTypeEnum.错误, showType = MessageShowType.右下角弹窗, MessageStr = $"日期时间格式不正确" };
                this._mediator.Publish(notice);
            }
        }

        #region 【声明】

        public readonly IMediator _mediator;
        public readonly IAPIHelper _apiHelper;

        public ICommand SearchHisData { get; }

        private int _PCount;

        public int PCount
        {
            get => _PCount;
            set
            {
                if (_PCount != value)
                {
                    _PCount = value;
                    OnPropertyChanged(nameof(PCount));
                }
            }
        }

        private DateTime _Date_Begin;

        public DateTime Date_Begin
        {
            get => _Date_Begin;
            set
            {
                if (_Date_Begin != value)
                {
                    _Date_Begin = value;
                    OnPropertyChanged(nameof(Date_Begin));
                }
            }
        }

        private DateTime _Date_End;
        public DateTime Date_End
        {
            get => _Date_End;
            set
            {
                if (_Date_End != value)
                {
                    _Date_End = value;
                    OnPropertyChanged(nameof(Date_End));
                }
            }
        }
        private DateTime _Time_Begin;
        public DateTime Time_Begin
        {
            get => _Time_Begin;
            set
            {
                if (_Time_Begin != value)
                {
                    _Time_Begin = value;
                    OnPropertyChanged(nameof(Time_Begin));
                }
            }
        }
        private DateTime _Time_End;
        public DateTime Time_End
        {
            get => _Time_End;
            set
            {
                if (_Time_End != value)
                {
                    _Time_End = value;
                    OnPropertyChanged(nameof(Time_End));
                }
            }
        }


        /// <summary>
        /// 任务数据源
        /// </summary>
        public ObservableCollection<PackTaskRecordDataDTO> ProductList
        {
            get => _ProductList;
            set
            {
                if (_ProductList != value)
                {
                    _ProductList = value;
                    OnPropertyChanged(nameof(ProductList));
                }
            }
        }
        private ObservableCollection<PackTaskRecordDataDTO> _ProductList;



        #endregion
    }
}
