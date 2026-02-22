using AsZero.Core.Services.Messages;
using DynamicData;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;

namespace Automatic.Client.ViewModels.Realtime
{
    public class UILogsViewModel : ReactiveObject, IDisposable
    {
        private IDisposable _cleanup;
        public UILogsViewModel()
        {
            CmdClearFilter = ReactiveCommand.Create(() =>
            {
                EventGroup = "";
            });

            var disposeCmdClearFilterException = CmdClearFilter.ThrownExceptions.Subscribe(x => {
            });


            CmdClear = ReactiveCommand.Create(() =>
            {
                _source.Clear();
            });
            var disposeCmdClear = CmdClear.ThrownExceptions.Subscribe(x => {
            });


            var eventgroupFilter = this.WhenAnyValue(x => x.EventGroup)
                .Throttle(TimeSpan.FromMilliseconds(400))
                .DistinctUntilChanged()
                .Select(x => {
                    Func<LogMessage, bool> res = lm => {
                        if (string.IsNullOrEmpty(x))
                        {
                            return true;
                        }
                        return lm.EventGroup == x;
                    };
                    return res;
                });

            ChangeObs = _source.Connect()
                .Filter(eventgroupFilter);

            var d = ChangeObs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Bind(out _logs)
                .DisposeMany()
                .Subscribe();

            _cleanup = new CompositeDisposable(
                d,
                disposeCmdClearFilterException,
                disposeCmdClear
            );

        }

        private SourceList<LogMessage> _source = new SourceList<LogMessage>();

        #region
        private readonly ReadOnlyObservableCollection<LogMessage> _logs;
        public ReadOnlyObservableCollection<LogMessage> Logs => _logs;
        public IObservable<IChangeSet<LogMessage>> ChangeObs { get; }
        #endregion

        #region
        [Reactive]
        public string EventGroup { get; set; }
        #endregion

        public void OnNext(LogMessage msg)
        {
            while (_source.Count > 1000)
            {
                _source.RemoveAt(0);
            }
            _source.Add(msg);
        }

        public ReactiveCommand<Unit, Unit> CmdClearFilter { get; }
        public ReactiveCommand<Unit, Unit> CmdClear { get; }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}
