using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using RogerTech.Common.AuthService;
using RogerTech.Common;
using RogerTech.BussnessCore;

namespace MPAssmebleRecipe.Apps
{
    public class AppManager
    {
        public IEventAggregator _eventAggregator;
        public static AppManager Instance { get; private set; }
        public static AppManager GetInstance(IEventAggregator eventAggregator)
        {
            if (Instance == null)
            {
                Instance = new AppManager(eventAggregator);
            }
            return Instance;
        }

        private AppManager(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;

            ///菜单消息
            eventAggregator.GetEvent<LogMessage>().Subscribe((arg) =>
            {

            });
        }
        ///切换用户事件
        public EventHandler OnUserChange;

        public BussnessCore bussness { get; private set; }

        public UserInfo UserInfo { get; set; } = null;
        private AppManager()
        {
            bussness = new BussnessCore();
        }

        private static AppManager instance;

        public static AppManager GetInstance()
        {
            if (instance == null)
            {
                instance = new AppManager();
            }
            return instance;
        }
    }
}
