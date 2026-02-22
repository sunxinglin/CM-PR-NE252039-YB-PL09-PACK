using Ctp0600P.Client.CommonEntity;
using Ctp0600P.Client.UserCtrls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ctp0600P.Client.Views.Pages
{
    /// <summary>
    /// StationTaskListUC.xaml 的交互逻辑
    /// </summary>
    public partial class StationTaskListPage : UserControl
    {
        public StationTaskListPage()
        {
            InitializeComponent();
        }

        //public StationTaskViewDTOList Db { get; set; }
        //private void StationTaskListPage_Loaded(object sender, RoutedEventArgs e)
        //{
        //    if (this.GridList.Children.Count == 0)
        //        AddRow();
        //}

        //public void PageReload()
        //{
        //    this.GridList.Children.Clear();
        //    AddRow();
        //}

        //private void AddRow()
        //{
        //    if (Db != null)
        //    {
        //        if (Db.Count != 0)
        //        {
        //            for (int i = 0; i < Db.Count; i++)
        //            {
        //                StationTaskHearderInfo infoUC = new StationTaskHearderInfo();
        //                var infostr = Db.Infos[i];
        //                bool arrowvisi = true;
        //                if (i == Db.Count - 1)
        //                {
        //                    arrowvisi = false;
        //                }
        //                var infovm = new StationTaskHearderInfoVm(infostr, infostr.StepNo == 0 ? Db.DoingColor : Db.UnFinishColor, arrowvisi);
        //                infoUC.DataContext = infovm;
        //                infoUC.Tag = Db.Infos[i].StepNo;
        //                this.GridList.Children.Add(infoUC);
        //                Grid.SetRow(infoUC, i);
        //            }
        //        }
        //    }
        //}
        ///// <summary>
        ///// 更改状态
        ///// </summary>
        ///// <param name="idx">序号</param>
        ///// <param name="status">状态(true:完成,false:未完成)</param>
        ///// <returns></returns>
        //public (bool, string) ChangeStatus(int idx, bool status)
        //{
        //    try
        //    {
        //        if (idx < 0)
        //        {
        //            return (false, $"序号{idx}不可小于0!");
        //        }

        //        if (idx >= GridList.Children.Count)
        //        {
        //            return (false, $"序号{idx}不可大于总步数!");
        //        }

        //        if (status)
        //        {
        //            for (var i = 0; i < GridList.Children.Count; i++)
        //            {
        //                var childer = (StationTaskHearderInfo)GridList.Children[i];
        //                if (childer != null)
        //                {
        //                    var childerVm = (StationTaskHearderInfoVm)childer.DataContext;
        //                    if (int.Parse(childer.Tag.ToString()) <= idx)
        //                    {
        //                        childerVm.BackBrush = Db.FinishColor;
        //                    }
        //                    else if (int.Parse(childer.Tag.ToString()) == idx + 1)
        //                    {
        //                        childerVm.BackBrush = Db.DoingColor;
        //                    }
        //                    else
        //                    {
        //                        childerVm.BackBrush = Db.UnFinishColor;
        //                    }
        //                }
        //            }
        //            return (true, "");
        //        }
        //        else
        //        {
        //            for (var i = 0; i < GridList.Children.Count; i++)
        //            {
        //                var childer = (StationTaskHearderInfo)GridList.Children[i];
        //                if (childer != null)
        //                {
        //                    var childerVm = (StationTaskHearderInfoVm)childer.DataContext;
        //                    if (int.Parse(childer.Tag.ToString()) < idx - 1)
        //                    {
        //                        childerVm.BackBrush = Db.FinishColor;
        //                    }
        //                    else if (int.Parse(childer.Tag.ToString()) == idx - 1)
        //                    {
        //                        childerVm.BackBrush = Db.DoingColor;
        //                    }
        //                    else
        //                    {
        //                        childerVm.BackBrush = Db.UnFinishColor;
        //                    }
        //                }
        //            }
        //            return (true, "");
        //        }
        //        return (false, $"序号为{idx}的对象为空,查看是否加入了该对象");
        //    }
        //    catch (Exception ex)
        //    {

        //        return (false, ex.Message);
        //    }
        //}

    }
}
