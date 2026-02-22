using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using RogerTech.AuthService.Models;
using RogerTech.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPAssmebleRecipe.Apps.ViewModels
{
    public class AuthMainViewModel : BindableBase, IRegionMemberLifetime
    {
        public bool KeepAlive => true;

        public AuthMainViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
       
            FuncList = new ObservableCollection<string>();
            FuncList.Add("用户列表");
            FuncList.Add("角色管理");
            //FuncList.Add("角色菜单");
            FuncList.Add("角色关系");
            this.regionManager = regionManager;
            var item = DbContext.GetInstance().Queryable<User>().ToList();
        }
        private ObservableCollection<string> funcList;

        public ObservableCollection<string> FuncList
        {
            get { return funcList; }
            set { funcList = value; RaisePropertyChanged(); }
        }

        private string selectFun;
        private readonly IRegionManager regionManager;

        public string SelectFun
        {
            get { return selectFun; }
            set
            {
                if (SetProperty(ref selectFun, value))
                {
                    NavigateToSelectedView(selectFun);
                }
            }
        }

        private void NavigateToSelectedView(string selectfun)
        {
            switch (selectFun)
            {
                case "用户列表":
                    regionManager.Regions["UserRegion"].RequestNavigate("UserView");
                    break;
                case "角色管理":
                    regionManager.Regions["UserRegion"].RequestNavigate("RoleView");
                    break;
                case "角色关系":
                    regionManager.Regions["UserRegion"].RequestNavigate("RoleManagerView");
                    break;
                default:
                    break;
            }
        }



    }
}
