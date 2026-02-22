using MPAssmebleRecipe.Models.Entities.Production;
using MPAssmebleRecipe.Models.Repositories;
using MPAssmebleRecipe.Models.Repositories.Production;
using MPAssmebleRecipe.Service;
using MPAssmebleRecipe.Service.IService;
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

namespace MPAssmebleRecipe.Apps.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IRecipeHelperService _recipeHelperService;
        private readonly IPackRepository _packRepository;
        private readonly IRecipeItemService _recipeItemService;
        public MainWindow(IRecipeHelperService recipeHelperService, IPackRepository packRepository, IRecipeItemService recipeItemService)
        {
            InitializeComponent();
            _recipeHelperService = recipeHelperService;
            _packRepository = packRepository;
            _recipeItemService = recipeItemService;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> cellSn = new List<string>() { "11", "22", "33", "44"};
            List<string> msg = new List<string>();
            _recipeItemService.GetRecipeItemsByCellSns(cellSn,out msg);
            foreach (var item in msg)
            {
                Console.WriteLine(item);
            }
        }
    }
}
