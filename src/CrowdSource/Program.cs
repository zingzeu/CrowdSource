using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using CrowdSource.Tools;

namespace CrowdSource
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            if (args.Length > 0)
            {
                CommandLineToolsLauncher.Main(host, args);
            }
            else
            {
                host.Run();
            }


        }
    }
}
