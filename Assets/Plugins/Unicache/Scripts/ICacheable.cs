namespace UnicacheCore
{
    public interface ICacheable
    {
        string CacheKey { get; }
        string CacheVersion { get; }
    }
}