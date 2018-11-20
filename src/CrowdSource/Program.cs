using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using CrowdSource.Tools;

namespace CrowdSource
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            if (args.Length > 0)
            {
                CommandLineToolsLauncher.MainAsync(host, args).Wait();
            }
            else
            {
                host.Run();
            }
        }
    }
}
