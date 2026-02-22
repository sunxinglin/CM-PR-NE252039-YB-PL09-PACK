
using Ctp0600P.Client.Command.NoticeHelp.Handle;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Ctp0600P.Client.Command
{
    public static class ServiceAdd
    {
        public static IServiceCollection AddCommand(this IServiceCollection services)
        {
            services.AddMediatR(
               typeof(MessageNoticeHandle).Assembly
            );
            return services;
        }
    }
}
