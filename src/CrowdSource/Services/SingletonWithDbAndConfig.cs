using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CrowdSource.Models.CoreModels;
using Microsoft.Extensions.Logging;
using System.Threading;
using CrowdSource.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CrowdSource.Services
{
    public abstract class SingletonWithDbAndConfig {
        private readonly IServiceScopeFactory _scopeFactory;

        public SingletonWithDbAndConfig(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected void RunWithDbContext(Action<ApplicationDbContext> action) {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                action(context);
            }
        }

        protected async Task RunWithDbContextAsync(Func<ApplicationDbContext, Task> action) {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                await action(context);
            }
        }
        protected void RunWithConfigContext(Action<IDbConfig> action) {
            using (var scope = _scopeFactory.CreateScope())
            {
                var config = scope.ServiceProvider.GetService<IDbConfig>();
                action(config);
            }
        }
    }
}