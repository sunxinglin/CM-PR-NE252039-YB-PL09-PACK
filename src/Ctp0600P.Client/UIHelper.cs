using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Ctp0600P.Client;

public static class UIHelper
{
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