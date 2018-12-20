using System;
using System.Threading.Tasks;

namespace Zezo.Core.GrainInterfaces
{
    public interface IEntity : Orleans.IGrainWithGuidKey
    {
        Task<IProject> GetProject();

        // Pause
        // Resume
        // Archive
        // Delete
    }

    public class EntityData {
        public enum EntityStatus {
            Active,
            Archived
        }
        public EntityStatus Status;

    }
}
