using System;
using System.Collections.Generic;
using System.Text;
using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CrowdSource.Tools.Commands
{
    public class AddUserToRoleCommand : ICommand
    {
        public void Run(string[] args)
        {
            Console.WriteLine("Add User To Role");
            if (args.Count() != 2)
            {
                Console.WriteLine("Invalid Arguments");
                return;
            }

            var context = new ApplicationDbContext(
                 new DbContextOptionsBuilder<ApplicationDbContext>()
                 .UseNpgsql("Server=127.0.0.1;Port=5432;Database=crowdsource;User Id=postgres;Password=root;")
            .Options);

            var user = args[0];
            var role = args[1];
            IdentityRole identityRole;
            if (!context.Roles.Any(r => r.Name == role ))
            {
                context.Roles.Add(identityRole = new IdentityRole(role));
            } else
            {
                identityRole = context.Roles.Single(r => r.Name == role);
            }

            var identityUser = context.Users.SingleOrDefault(u => u.Email == user);
            if (identityUser == null)
            {
                Console.WriteLine("No Such User: " + user);
                return;
            }
            context.UserRoles.Add(new IdentityUserRole<string>())
            context.SaveChanges();
        }


    }
}
