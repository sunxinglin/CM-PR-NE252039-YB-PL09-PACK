using Prism.Events;
using RogerTech.Common;
using RogerTech.Common.AuthService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Menu = RogerTech.AuthService.Models.Menu;

namespace MPAssmebleRecipe.Apps.Views.PackManage
{
    /// <summary>
    /// Pack管理视图
    /// </summary>
    public partial class UcPackManageView : UserControl
    {
       // SqlSugarClient db = DbContext.GetInstance().db;
        public UcPackManageView(IEventAggregator eventAggregator)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            InitializeComponent();
            Console.WriteLine(sw.ElapsedMilliseconds);
            EventAggregator = eventAggregator;
            AppManager appManager = AppManager.GetInstance();
            ChangUser(appManager.UserInfo);
            EventAggregator.GetEvent<UserInfoEvent>().Subscribe(user =>
            {
                ChangUser(user);
            });

            RegistMenus();
        }

        private void RegistMenus()
        {
            List<Menu> menuList = new List<Menu>()
            {
                new Menu() { Page = "UcPackManageView", SubPage = "",ElementName="添加PACK" },
                new Menu() { Page = "UcPackManageView", SubPage = "",ElementName="删除PACK" },
                new Menu() { Page = "UcPackManageView", SubPage = "",ElementName="添加BLOCK" },
                new Menu() { Page = "UcPackManageView", SubPage = "",ElementName="删除BLOCK" },
                new Menu() { Page = "UcPackManageView", SubPage = "",ElementName="维护Block" },
                new Menu() { Page = "UcPackManageView", SubPage = "",ElementName="维护Unit" },
                new Menu() { Page = "UcPackManageView", SubPage = "",ElementName="维护电芯" }
            };

          //  List<Menu> menuList = DbContext.GetInstance().Queryable<Menu>().Where(p => p.Page == "UcPackManageView" && p.SubPage == "B").ToList();
            RogerTech.AuthService.AuthService authService = new RogerTech.AuthService.AuthService();
            foreach (var item in menuList)
            {
                authService.AddMenu(item);
            }
        }

        public IEventAggregator EventAggregator { get; }

        private void ChangUser(UserInfo user)
        {
            if (user == null)
            {
                添加PACK.IsEnabled = false;
                删除PACK.IsEnabled = false;
                添加BLOCK.IsEnabled = false;
                删除BLOCK.IsEnabled = false;
                维护Block.IsEnabled = false;
                维护Unit.IsEnabled = false;
                维护电芯.IsEnabled = false;
                return;
            }
            var addUser = user.UserMenus.Where(x => x.Page == "UcPackManageView" && x.ElementName == "添加PACK").FirstOrDefault();
            添加PACK.IsEnabled = addUser != null ? true : false;
            addUser = user.UserMenus.Where(x => x.Page == "UcPackManageView" && x.ElementName == "删除PACK").FirstOrDefault();
            删除PACK.IsEnabled = addUser != null ? true : false;
            addUser = user.UserMenus.Where(x => x.Page == "UcPackManageView" && x.ElementName == "添加BLOCK").FirstOrDefault();
            添加BLOCK.IsEnabled = addUser != null ? true : false; 
            addUser = user.UserMenus.Where(x => x.Page == "UcPackManageView" && x.ElementName == "删除BLOCK").FirstOrDefault();
            删除BLOCK.IsEnabled = addUser != null ? true : false;
            addUser = user.UserMenus.Where(x => x.Page == "UcPackManageView" && x.ElementName == "维护Block").FirstOrDefault();
            维护Block.IsEnabled = addUser != null ? true : false;
            addUser = user.UserMenus.Where(x => x.Page == "UcPackManageView" && x.ElementName == "维护Unit").FirstOrDefault();
            维护Unit.IsEnabled = addUser != null ? true : false;
            addUser = user.UserMenus.Where(x => x.Page == "UcPackManageView" && x.ElementName == "维护电芯").FirstOrDefault();
            维护电芯.IsEnabled = addUser != null ? true : false;
        }
    }
}