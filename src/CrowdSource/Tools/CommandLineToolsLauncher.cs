using System;
using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using CrowdSource.Models.CoreModels;
using CrowdSource.Tools.Commands;
using Microsoft.AspNetCore.Hosting;

namespace CrowdSource.Tools
{
    class CommandLineToolsLauncher
    {
        public static void Main(IWebHost host, string[] args)
        {
            String command = "";
            if (args.Count() > 0)
            {
                command = args[0];
            }

            if (command == "SetUserRole")
            {
                var runner = new AddUserToRoleCommand();
                runner.Run(host, args.Skip(1).ToArray());
            }
            else if (command == "SeedDb")
            {
                var runner = new SeedDbCommand();
                runner.Run(host, args.Skip(1).ToArray());
            }
            else
            {
                Console.WriteLine("Command Not Found.");
            }

            Console.ReadLine();
        }
    }
}
