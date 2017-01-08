namespace UnicacheCore
{
    // CacheLocator determinates save path from resource url
    public interface ICacheLocator
    {
        string CreatePath(string url);
    }
}