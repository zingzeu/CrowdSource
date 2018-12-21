using System;
using System.Threading.Tasks;
using CrowdSource.GrainInterfaces;

namespace CrowdSource.Grains
{
    public class HelloGrain : Orleans.Grain, IHello
    {
        Task<string> IHello.SayHello(string msg)
        {
            return Task.FromResult($"You said {msg}, I say: Hello!");
        }
    }
}
