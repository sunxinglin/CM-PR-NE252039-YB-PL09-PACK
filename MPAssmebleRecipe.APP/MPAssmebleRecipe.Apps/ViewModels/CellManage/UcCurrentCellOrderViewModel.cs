using MPAssmebleRecipe.Logger.Interfaces;
using RogerTech.Common.Models;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using RogerTech.Common;

namespace MPAssmebleRecipe.Apps.ViewModels.CellManage
{
    public class UcCurrentCellOrderViewModel : BindableBase
    {
        private readonly ILoggerHelper _logger;

        /// <summary>
        /// 电芯列表
        /// </summary>
        private ObservableCollection<Production_Recipe> _cellItems;
        public ObservableCollection<Production_Recipe> CellItems
        {
            get => _cellItems;
            set => SetProperty(ref _cellItems, value);
        }

        public UcCurrentCellOrderViewModel(ILoggerHelper logger)
        {
            _logger = logger;
            // 初始化数据
            InitializeAsync();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private async void InitializeAsync()
        {
            try
            {
                await LoadCellItems();
                // 启动定时刷新
                StartRefreshTimer();
            }
            catch (Exception ex)
            {
               // LogHelper.Errors("初始化电芯下发数据失败");
                MessageBox.Show($"加载数据失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 加载电芯数据
        /// </summary>
        private async Task LoadCellItems()
        {
            try
            {
                List<Production_Recipe> result;
                //var lastRecipeItem = DbContext.GetInstance().Queryable<Production_Recipe>().AS("recipe_temp").OrderByDescending(x => x.ID).First();
                var lastRecipeItem = DbContext.GetInstance().Queryable<Production_Recipe>().OrderByDescending(x => x.ID).First();
                if (lastRecipeItem == null)
                {
                    result= new List<Production_Recipe>();
                }
                //result = DbContext.GetInstance().Queryable<Production_Recipe>().AS("recipe_temp").Where(x => x.Uid == lastRecipeItem.Uid).ToList();
                //result = DbContext.GetInstance().Queryable<Production_Recipe>().Where(x => x.Uid == lastRecipeItem.Uid).ToList();

                //var cells = result;
                //CellItems = new ObservableCollection<Production_Recipe>(cells.OrderBy(c => c.DownLoadIndex));
            }
            catch (Exception ex)
            {
              //  LogHelper.Errors("加载电芯下发数据失败");
                throw;
            }
        }

        /// <summary>
        /// 启动定时刷新
        /// </summary>
        private void StartRefreshTimer()
        {
            // 创建定时器，每5秒刷新一次
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            timer.Tick += async (s, e) => await LoadCellItems();
            timer.Start();
        }
    }
}