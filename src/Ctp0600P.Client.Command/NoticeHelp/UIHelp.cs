using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Ctp0600P.Client.Command.NoticeHelp;
using Ctp0600P.Client.Command.NoticeHelp.Enum;

namespace SDKSD.Client.Commom;

internal static class UIHelp
{

    public static List<NotificationWindow> _dialogs = new();


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
            var MessagePn = new StackPanel
            {
                Height = 80,
                Width = double.NaN
            };

            var header = new TextBlock
            {
                Text = MsgType,
                FontSize = 18,
                FontWeight = FontWeights.Bold
            };
            MessagePn.Children.Add(header);
            var content = new TextBlock
            {
                Text = MsgComment,
                Width = double.NaN,
                FontSize = 30
            };
            MessagePn.Children.Add(content);
            return XamlWriter.Save(MessagePn);
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
        var myRectangle = new Rectangle
        {
            Width = 100,
            Height = 100
        };

        var MessagePn = new StackPanel
        {
            Height = 80,
            // 20号字体,在1920*1080分辨率下单字大小为22像素,为了显示全面,按照字体数量+1显示文本
            Width = (MsgComment.Length+1) *22,
            Margin = new Thickness(20)
        };

        var header = new TextBlock
        {
            Text = MsgType,
            FontSize = 20,
            FontWeight = FontWeights.Bold
        };
        MessagePn.Children.Add(header);
        var content = new TextBlock
        {
            Text = MsgComment,
            Width = double.NaN,
            FontSize = 25
        };
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
        return XamlWriter.Save(MessagePn);
    }

    /// <summary>
    /// 弹框
    /// </summary>
    /// <param name="type"></param>
    /// <param name="content"></param>
    public static void ShowNotice(MessageTypeEnum type, string content)
    {

        RunInUIThread(o =>
        {
            var data = new NotifyData
            {
                Title = type.ToString(),
                Content = content
            };
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

            // new一个通知
            var dialog = new NotificationWindow();
            dialog.Closed += Dialog_Closed;
            dialog.TopFrom = GetTopFrom();
            dialog.Topmost = true;
            _dialogs.Add(dialog);
            // 设置通知里要显示的数据
            dialog.DataContext = data;
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
        double topFrom = SystemParameters.WorkArea.Bottom - 10;
        bool isContinueFind = _dialogs.Any(o => o.TopFrom == topFrom);

        while (isContinueFind)
        {
            topFrom = topFrom - 110;//此处100是NotifyWindow的高 110-100剩下的10  是通知之间的间距
            isContinueFind = _dialogs.Any(o => o.TopFrom == topFrom);
        }

        if (topFrom <= 0)
            topFrom = SystemParameters.WorkArea.Bottom - 10;

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
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null)
            {
                SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(dispatcher));
                SynchronizationContext.Current?.Post(callback, null);
            }
        });
    }

}