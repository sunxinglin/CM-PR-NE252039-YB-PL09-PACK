using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Yee.Services.ProductionRecord;

namespace Ctp0600P.Client.Apis
{
    public static class ApiServiceCollectionExtensions
    {
        public static void AddClientApis(this IServiceCollection services)
        {
            services.AddSingleton<APIHelper>();
            services.AddSingleton<CatlMesIniConfigApi>();
        }
    }
}
