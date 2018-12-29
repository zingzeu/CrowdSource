namespace Zezo.Core.Datastores.SimpleStore
{
    public interface ISimpleStoreProxy
    {
        dynamic this[string key] { get; set; }
    }
    
    public class ChangeRecord
    {
        public string Name { get; set; }
        public dynamic NewValue { get; set; }
    }
}