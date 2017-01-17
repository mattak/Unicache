using Unicache;

namespace UnicacheExample.Version
{
    public class VersionUrlLocator : IUrlLocator
    {
        public string CreateUrl(string key)
        {
            var url = string.Format("https://raw.githubusercontent.com/mattak/Unicache/master/art/{0}.png", key);
            return url;
        }
    }
}