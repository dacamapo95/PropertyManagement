namespace PropertyManagement.Infrastructure.Caching;

public sealed class CacheOptions
{
    public EntityCacheOptions City { get; set; } = new();
    public EntityCacheOptions State { get; set; } = new();
}

public sealed class EntityCacheOptions
{
    public bool Enabled { get; set; } = true;
    public int? AbsoluteExpirationSeconds { get; set; }
    public int? SlidingExpirationSeconds { get; set; }
}
