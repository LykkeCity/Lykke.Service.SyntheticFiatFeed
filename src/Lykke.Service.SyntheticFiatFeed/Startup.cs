using System;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.SyntheticFiatFeed.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.SyntheticFiatFeed
{
    [UsedImplicitly]
    public class Startup
    {
        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {                                   
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.ApiTitle = "SyntheticFiatFeed API";
                options.Logs = logs =>
                {
                    logs.AzureTableName = "SyntheticFiatFeedLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.SyntheticFiatFeedService.Db.LogsConnString;

                    // TODO: You could add extended logging configuration here:
                    /* 
                    logs.Extended = extendedLogs =>
                    {
                        // For example, you could add additional slack channel like this:
                        extendedLogs.AddAdditionalSlackChannel("SyntheticFiatFeed", channelOptions =>
                        {
                            channelOptions.MinLogLevel = LogLevel.Information;
                        });
                    };
                    */
                };

                // TODO: You could add extended Swagger configuration here:
                /*
                options.Swagger = swagger =>
                {
                    swagger.IgnoreObsoleteActions();
                };
                */
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration();

        }
    }
}
