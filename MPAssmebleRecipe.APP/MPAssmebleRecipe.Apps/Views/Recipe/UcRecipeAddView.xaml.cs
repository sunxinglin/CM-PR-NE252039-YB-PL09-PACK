using MPAssmebleRecipe.Logger.Interfaces;
using Prism.Events;
using Prism.Ioc;
using RogerTech.Common;
using RogerTech.Common.AuthService;
using RogerTech.Common.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Menu = RogerTech.AuthService.Models.Menu;

namespace MPAssmebleRecipe.Apps.Views.Recipe
{
    /// <summary>
    /// UcRecipeManageView.xaml 的交互逻辑
    /// </summary>
    public partial class UcRecipeAddView : UserControl
    {
        public UcRecipeAddView()
        {
            InitializeComponent();
        }
    }
}
