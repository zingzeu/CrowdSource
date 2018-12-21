using System;
using Orleans;
using Orleans.Hosting;
using Orleans.Runtime.Configuration;
using CrowdSource.Grains;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CrowdSource.SiloHost
{
    class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var config = ClusterConfiguration.LocalhostPrimarySilo();
            config.AddMemoryStorageProvider();

            var builder = new SiloHostBuilder()
                .UseConfiguration(config)
                .ConfigureApplicationParts(
                    parts => parts.AddApplicationPart(typeof(HelloGrain).Assembly)
                        .WithReferences()
                )
                .ConfigureLogging(logging => logging.AddConsole());

            var host = builder.Build();
            await host.StartAsync();
            return host;

            
        }
    }
}
