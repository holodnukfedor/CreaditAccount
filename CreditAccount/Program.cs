using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using CreditAccountDAL;

namespace CreditAccount
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();
            DatabaseDeployer databaseDeployer = (DatabaseDeployer) host.Services.GetService(typeof(DatabaseDeployer));
            databaseDeployer.DeployAsync().Wait();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
