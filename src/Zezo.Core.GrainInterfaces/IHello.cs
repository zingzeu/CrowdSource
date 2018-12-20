using System;
using System.Threading.Tasks;

namespace Zezo.Core.GrainInterfaces
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }
}
