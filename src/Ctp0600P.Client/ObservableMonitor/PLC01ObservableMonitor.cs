using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Ctp0600P.Client.PLC.PLC01;

using Microsoft.Extensions.Logging;

namespace Ctp0600P.Client.ObservableMonitor;

public class PLC01ObservableMonitor
{
    private readonly Subject<ScanContext> _contextSubject = new();
    private readonly Subject<DateTimeOffset> _heartBeatedSubject = new();

    public IObservable<ScanContext> ContextSource { get; }

    public IObservable<DateTimeOffset> HeartBeatAckedAt { get; }

    public PLC01ObservableMonitor(ILoggerFactory loggerFactory)
    {
        ContextSource = _contextSubject.AsObservable();
        HeartBeatAckedAt = _heartBeatedSubject.AsObservable();
    }

    public void OnNextHearted(DateTimeOffset dt)
    {
        _heartBeatedSubject.OnNext(dt);
    }

    public void OnNextContext(ScanContext ctx)
    {
        _contextSubject.OnNext(ctx);
    }
}