using System;
using System.Threading.Tasks;

namespace Zezo.Core.GrainInterfaces
{
    public interface IUser : Orleans.IGrainWithGuidKey
    {
        Task<IProject> GetProject();
    }
}
