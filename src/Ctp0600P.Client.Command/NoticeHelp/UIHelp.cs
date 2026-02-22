using Ctp0600P.Client.Command.NoticeHelp;
using Ctp0600P.Client.Command.NoticeHelp.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SDKSD.Client.Commom
{
    internal static class UIHelp
    {

        public static List<NotificationWindow> _dialogs = new List<NotificationWindow>();


        /// <summary>
        /// Rubyer的基本用法需要传xaml 
        /// </summary>
        /// <param name="MsgType"></param>
        /// <param name="MsgComment"></param>
        /// <returns></returns>
        public static string GetMessageBoxXaml(string MsgType, string MsgComment)
        {
            try
            {
                StackPanel MessagePn = new StackPanel();
                MessagePn.Height = 80;
                MessagePn.Width = double.NaN;

                TextBlock header = new TextBlock();
                header.Text = MsgType;
                header.FontSize = 18;
                header.FontWeight = FontWeights.Bold;
                MessagePn.Children.Add(header);
                TextBlock content = new TextBlock();
                content.Text = MsgComment;
                content.Width = double.NaN;
                content.FontSize = 30;
                MessagePn.Children.Add(content);
                return System.Windows.Markup.XamlWriter.Save(MessagePn);
            }
            catch (Exception ex)
            {

                throw new Exception($"出错啦{ex.Message}");
            }
        }


        /// <summary>
        /// Rubyer的基本用法需要传xaml 
        /// </summary>
        /// <param name="MsgType"></param>
        /// <param name="MsgComment"></param>
        /// <returns></returns>
        public static string GetNoticeXaml(string MsgType, string MsgComment)
        {
            Rectangle myRectangle = new Rectangle();
            myRectangle.Width = 100;
            myRectangle.Height = 100;

            StackPanel MessagePn = new StackPanel();

            MessagePn.Height = 80;
            //20号字体,在1920*1080分辨率下单字大小为22像素,为了显示全面,按照字体数量+1显示文本
            MessagePn.Width = (MsgComment.Length+1) *22;
            MessagePn.Margin = new Thickness(20);
            TextBlock header = new TextBlock();
            header.Text = MsgType;
            header.FontSize = 20;
            header.FontWeight = FontWeights.Bold;
            MessagePn.Children.Add(header);
            TextBlock content = new TextBlock();
            content.Text = MsgComment;
            content.Width = double.NaN;
            content.FontSize = 25;
            if (MessagePn.Children.Contains(content))
            {
                MessagePn.Children.Remove(content);
                MessagePn.Children.Remove(myRectangle);

            }
            else
            {
                MessagePn.Children.Add(content);
                MessagePn.Children.Add(myRectangle);
            }
            return System.Windows.Markup.XamlWriter.Save(MessagePn);
        }

        /// <summary>
        /// 弹框
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        public static void ShowNotice(MessageTypeEnum type, string content)
        {

            UIHelp.RunInUIThread(o =>
            {
                NotifyData data = new NotifyData();
                data.Title = type.ToString();
                data.Content = content;
                if (type == MessageTypeEnum.提示)
                {
                    data.Background = new SolidColorBrush(Colors.LightSkyBlue);
                }
                else if (type == MessageTypeEnum.警告)
                {
                    data.Background = new SolidColorBrush(Colors.Orange);
                }
                else if (type == MessageTypeEnum.错误)
                {
                    data.Background = new SolidColorBrush(Colors.DarkRed);
                }

                NotificationWindow dialog = new NotificationWindow();//new 一个通知
                dialog.Closed += Dialog_Closed;
                dialog.TopFrom = GetTopFrom();
                dialog.Topmost = true;
                _dialogs.Add(dialog);
                dialog.DataContext = data;//设置通知里要显示的数据
                dialog.Show();

            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Dialog_Closed(object sender, EventArgs e)
        {
            NotificationWindow closedDialog = sender as NotificationWindow;
            _dialogs.Remove(closedDialog);
            GC.Collect();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static double GetTopFrom()
        {
            //屏幕的高度-底部TaskBar的高度。
            double topFrom = System.Windows.SystemParameters.WorkArea.Bottom - 10;
            bool isContinueFind = _dialogs.Any(o => o.TopFrom == topFrom);

            while (isContinueFind)
            {
                topFrom = topFrom - 110;//此处100是NotifyWindow的高 110-100剩下的10  是通知之间的间距
                isContinueFind = _dialogs.Any(o => o.TopFrom == topFrom);
            }

            if (topFrom <= 0)
                topFrom = System.Windows.SystemParameters.WorkArea.Bottom - 10;

            return topFrom;
        }

        /// <summary>
        /// 供跨线程调用
        /// </summary>
        /// <param name="callback"></param>
        public static void RunInUIThread(SendOrPostCallback callback)
        {
            _ = Task.Run(() =>
            {
                var dispatcher = System.Windows.Application.Current?.Dispatcher;
                if (dispatcher != null)
                {
                    SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
                    SynchronizationContext.Current?.Post(callback, null);
                }
            });
        }

    }

   
}
