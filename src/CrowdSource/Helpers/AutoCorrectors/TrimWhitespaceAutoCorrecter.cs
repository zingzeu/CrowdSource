namespace CrowdSource.Helpers.AutoCorrectors 
{
    public class TrimWhitespaceAutoCorrector : IAutoCorrector {
        public string Apply(string input) {
            return input.Trim();
        }
    }
}