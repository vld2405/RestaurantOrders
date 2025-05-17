using Microsoft.Extensions.Configuration;
using RestaurantOrders.Infrastructure.Config;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace RestaurantOrders
{
    public partial class App : Application
    {
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
            AppConfig.Init(Configuration);
            base.OnStartup(e);
        }

    }
}
