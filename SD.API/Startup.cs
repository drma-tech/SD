using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SD.API.Core;
using SD.API.Repository;

[assembly: FunctionsStartup(typeof(SD.API.Startup))]

namespace SD.API
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = builder.GetContext().Configuration;

            //enable return null
            builder.Services.AddMvcCore().AddMvcOptions(options =>
            {
                options.OutputFormatters.RemoveType<HttpNoContentOutputFormatter>();
            });

            //builder.Services.AddHttpClient();

            builder.Services.AddSingleton<IRepository>((s) =>
            {
                return new CosmosRepository(config);
            });

            builder.Services.AddLogging(logging =>
            {
                logging.AddProvider(new CosmosLoggerProvider(new Repository.CosmosLogRepository(config)));
                //logging.AddAzureWebAppDiagnostics();
            });
        }
    }
}