using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Zezo.Core.Grains;

namespace Zezo.Silo
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync() 
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("\n\n Press Enter to terminate...\n\n");
                Console.ReadLine();
                await host.StopAsync();
                return 0;
            } catch (Exception ex) {
                Console.WriteLine(ex); 
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo() {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options => {
                    options.ClusterId = "dev";
                    options.ServiceId = "Zezo";
                })
                .ConfigureApplicationParts(parts =>
                    parts.AddApplicationPart(typeof(HelloGrain).Assembly)
                    .WithReferences())
                .ConfigureLogging(logging => 
                    logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}
