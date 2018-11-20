using System;
using System.Collections.Generic;
using System.Text;
using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using CrowdSource.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace CrowdSource.Tools.Commands
{
    public class RemoveUserFromRoleCommand : ICommand
    {
        public async Task RunAsync(IWebHost host, string[] args)
        {
            Console.WriteLine("Removing User From Role");

            if (args.Count() != 2)
            {
                Console.WriteLine("Invalid Arguments");
                return;
            }

            var user = args[0];
            var role = args[1];

            Console.WriteLine($"Removing User<{user}> From Role \"{role}\"");

            var services = (IServiceScopeFactory)host.Services.GetService(typeof(IServiceScopeFactory));
            using (var scope = services.CreateScope())
            {
                var @userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
               
                var idUser = await @userManager.FindByEmailAsync(user);

                if (idUser == null)
                {
                    Console.WriteLine("No Such User: " + user);
                    return;
                }
                
                try
                {
                    IdentityResult x = await @userManager.RemoveFromRoleAsync(idUser, role);
                    if (!x.Succeeded)
                    {
                        Console.WriteLine("Errors Removing User from Role.");
                        foreach (var error in x.Errors)
                        {
                            Console.WriteLine("[%s] %s", error.Code, error.Description);
                        }
                        return;
                    }
                    Console.WriteLine("Succeeded!");
                } 
                catch (Exception e)
                {
                    Console.WriteLine("Errors Removing User from Role.");
                    Console.WriteLine(e.Message);
                }

            }

        }

        private async Task EnsureRoleExists(string name, RoleManager<IdentityRole> roleManager)
        {
           
            if (!(await roleManager.RoleExistsAsync(name)))
            {
                Console.WriteLine($"Role {name} does not exist. Creating one...");
                await roleManager.CreateAsync(new IdentityRole(name));
            }
        }
    }
}
