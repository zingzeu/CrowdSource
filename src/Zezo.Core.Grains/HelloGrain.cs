using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zezo.Core.GrainInterfaces;
using Orleans;

namespace Zezo.Core.Grains
{
    public class HelloGrain : Orleans.Grain, IHello
    {
        private readonly ILogger logger;

        public HelloGrain(ILogger<HelloGrain> logger)
        {
            this.logger = logger;
        }

        public Task<string> SayHello(string greeting)
        {
            logger.LogInformation($"\nSaying hello, greeting='{greeting}'\n");
            return Task.FromResult($"Hello, {greeting}! from grain #{this.GetPrimaryKeyLong()}.");
        }
    }
}
