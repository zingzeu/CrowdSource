namespace CrowdSource.Helpers.AutoCorrectors 
{
    public interface IAutoCorrector {
        string Apply(string input);
    }
}