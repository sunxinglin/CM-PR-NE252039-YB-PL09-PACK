using Ctp0600P.Client.PLC.PLC01;
using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Ctp0600P.Client.ObservableMonitor
{
    public class PLC01ObservableMonitor
    {
        private Subject<ScanContext> _contextSubject = new();
        private Subject<DateTimeOffset> _heartbeatedSubject = new();

        private IObservable<ScanContext> _obsContextSource;
        private IObservable<DateTimeOffset> _observableHeartbeatSubject;

        public PLC01ObservableMonitor(ILoggerFactory loggerFactory)
        {
            this._obsContextSource = _contextSubject.AsObservable();
            this._observableHeartbeatSubject = _heartbeatedSubject.AsObservable();
        }

        public void OnNextHearted(DateTimeOffset dt)
        {
            this._heartbeatedSubject.OnNext(dt);
        }

        public void OnNextContext(ScanContext ctx)
        {
            this._contextSubject.OnNext(ctx);
        }


        public IObservable<DateTimeOffset> HeartBeatAckedAt => _observableHeartbeatSubject;
        public IObservable<ScanContext> ContextSource => _obsContextSource;


    }
}
