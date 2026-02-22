using AsZero.Core.Services.Messages;
using Ctp0600P.Client.Protocols;
using Ctp0600P.Shared;
using MediatR;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yee.Entitys.AlarmMgmt;

namespace Ctp0600P.Client.ViewModels
{
    public abstract class PagedVMBase<TRow, TApi>
    {
        public virtual TApi Api { get; }
        protected readonly IMediator _mediator;

        protected abstract Task<TableResp<TRow>> LoadTableAsync();

        public PagedVMBase(TApi api, IMediator mediator)
        {
            this.Api = api;
            this._mediator = mediator;
            this.PageIndex = new ReactiveProperty<int>(1);
            this.PageSize = new ReactiveProperty<int>(10);
            this.TableResp = new ReactiveProperty<TableResp<TRow>>(new TableResp<TRow>());
            this.TotalPages = this.TableResp.Select(t => (int)Math.Ceiling(t.Count * 1.0 / this.PageSize.Value)).ToReactiveProperty();

            this.CurrentSelected = new ReactiveProperty<TRow>();

            this.Records = this.TableResp.Select(i => i.Data).ToReactiveProperty();

            this.CmdLoad = new ReactiveCommand().WithSubscribe(async () => {
                try
                {
                    var table = await this.LoadTableAsync();
                    this.TableResp.Value = table;
                }
                catch (Exception ex)
                {
                    await this.PublishErrorNotification(ex);
                }
            });

            this.CmdNextPage = this.PageIndex.CombineLatest(this.TotalPages)
                .Select(arg => {
                    var (fst, snd) = arg;
                    return fst < snd;
                })
                .ToReactiveCommand()
                .WithSubscribe(async () => {
                    try
                    {
                        this.PageIndex.Value = this.PageIndex.Value + 1;
                        var table = await this.LoadTableAsync();
                        this.TableResp.Value = table;
                    }
                    catch (Exception ex)
                    {
                        await this.PublishErrorNotification(ex);
                    }
                });

            this.CmdPrevPage = this.PageIndex.Select(i => i > 1).ToReactiveCommand()
                .WithSubscribe(async () => {
                    try
                    {
                        var page = this.PageIndex.Value - 1;
                        page = page < 1 ? 1 : page;
                        this.PageIndex.Value = page;
                        var table = await this.LoadTableAsync();
                        this.TableResp.Value = table;
                    }
                    catch (Exception ex)
                    {
                        await this.PublishErrorNotification(ex);
                    }
                });

        }

        protected virtual async Task PublishErrorNotification(Exception ex)
        {
            var src = $"{typeof(TApi)}";
            await _mediator.Publish(new AlarmSYSNotification() { Code = AlarmCode.系统运行错误, Name = AlarmCode.系统运行错误.ToString(), Module = AlarmModule.DESOUTTER_MODULE, Description = $"加载{typeof(TRow)}：{ex.Message}\r\n{ex.StackTrace}\r\n 来源{src}" });
            
        }

        public virtual ReactiveProperty<int> PageIndex { get; }
        public virtual ReactiveProperty<int> PageSize { get; }
        public virtual ReactiveProperty<int> TotalPages { get; set; }
        public virtual ReactiveCommand CmdLoad { get; }
        public virtual ReactiveCommand CmdPrevPage { get; }
        public virtual ReactiveCommand CmdNextPage { get; }

        public virtual ReactiveProperty<TableResp<TRow>> TableResp { get; }
        public virtual ReactiveProperty<List<TRow>> Records { get; private set; }
        public virtual ReactiveProperty<TRow> CurrentSelected { get; }
    }
}
