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

namespace CrowdSource.Tools.Commands
{
    public class AddUserToRoleCommand : ICommand
    {
        public void Run(IWebHost host, string[] args)
        {
            Console.WriteLine("Add User To Role");

            if (args.Count() != 2)
            {
                Console.WriteLine("Invalid Arguments");
                return;
            }

            var user = args[0];
            var role = args[1];

            Console.WriteLine($"Adding User<{user}> To Role \"{role}\"");
            return;

            var services = (IServiceScopeFactory)host.Services.GetService(typeof(IServiceScopeFactory));
            using (var scope = services.CreateScope())
            {/*
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!context.Roles.Any(r => r.Name == role))
                {
                    context.Roles.Add( identityRole = new IdentityRole(role));
                }
                else
                {
                    identityRole = context.Roles.Single(r => r.Name == role);
                }

                var identityUser = context.Users.SingleOrDefault(u => u.Email == user);
                if (identityUser == null)
                {
                    Console.WriteLine("No Such User: " + user);
                    return;
                }
                context.UserRoles.Add(new IdentityUserRole<string>());
                context.SaveChanges();*/
            }

        }


    }
}
