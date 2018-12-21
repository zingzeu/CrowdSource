using System;
using System.Threading.Tasks;

namespace CrowdSource.GrainInterfaces
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string msg);
    }
}
