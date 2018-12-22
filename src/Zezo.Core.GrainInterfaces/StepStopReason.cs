namespace Zezo.Core.GrainInterfaces
{
    public enum StepStopReason
    {
        Completed = 0,
        
        ///<summary>Stopped due to internal error within the Step</summary>
        Error = 1,
        ///<summary>Aborted externally (by parent Step, or manually)</summary>
        Aborted = 2
    }
}