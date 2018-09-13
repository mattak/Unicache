namespace Unicache.Plugin
{
    public class SimpleUrlLocator : IUrlLocator
    {
        public string CreateUrl(string key)
        {
            return key;
        }
    }
}