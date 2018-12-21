using System;
using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using CrowdSource.Models.CoreModels;
using CrowdSource.Tools.Commands;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace CrowdSource.Tools
{
    class CommandLineToolsLauncher
    {
        public static async Task MainAsync(IWebHost host, string[] args)
        {
            String command = "";
            if (args.Count() > 0)
            {
                command = args[0];
            }

            if (command == "SetUserRole")
            {
                var runner = new AddUserToRoleCommand();
                await runner.RunAsync(host, args.Skip(1).ToArray());
            }
            else if (command == "SeedDb")
            {
                var runner = new SeedDbCommand();
                await runner.RunAsync(host, args.Skip(1).ToArray());
            }
            else if (command == "ImportEnglishText")
            {
                var runner = new ImportEnglishTextCommand();
                await runner.RunAsync(host, args.Skip(1).ToArray());
            }
            else
            {
                Console.WriteLine("Command Not Found.");
            }

            Console.ReadLine();
        }
    }
}
