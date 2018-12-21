namespace Zezo.Core.GrainInterfaces
{
    public class EntityGrainData {
        public enum EntityStatus {
            Active,
            Archived
        }
        public EntityStatus Status;

    }
}
