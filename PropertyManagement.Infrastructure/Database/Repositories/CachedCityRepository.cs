using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Infrastructure.Caching;
using System.Linq.Expressions;

namespace PropertyManagement.Infrastructure.Database.Repositories;

internal sealed class CachedCityRepository(
    ICityRepository inner,
    IMemoryCache cache,
    IOptions<CacheOptions> options) : ICityRepository
{
    private readonly ICityRepository _repository = inner;
    private readonly IMemoryCache _cache = cache;
    private readonly EntityCacheOptions _options = options.Value.City;


    public async Task<IReadOnlyList<City>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled) return await _repository.GetAllAsync(cancellationToken);

        var key = "City:All";

        if (_cache.TryGetValue(key, out IReadOnlyList<City>? cities) && cities is not null)
            return cities;

        cities = await _repository.GetAllAsync(cancellationToken);
        _cache.Set(key, cities, BuildEntryOptions());
        return cities;
    }

    public async Task<IReadOnlyList<City>> GetByStateIdAsync(Guid stateId, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled) return await _repository.GetByStateIdAsync(stateId, cancellationToken);

        var key = $"City:ByState:{stateId}";

        if (_cache.TryGetValue(key, out IReadOnlyList<City>? cities) && cities is not null)
            return cities;

        cities = await _repository.GetByStateIdAsync(stateId, cancellationToken);
        _cache.Set(key, cities, BuildEntryOptions());
        return cities;
    }

    public void DisableTracking() => _repository.DisableTracking();

    public async Task<City?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
      => await _repository.GetByIdAsync(id, cancellationToken);

    public async Task<IReadOnlyList<City>> GetAsync(Expression<Func<City, bool>> filter, CancellationToken cancellationToken = default)
        => await _repository.GetAsync(filter, cancellationToken);

    public async Task<bool> ExistsAsync(Expression<Func<City, bool>> filter, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(filter, cancellationToken);

    public async Task<int> CountAsync(Expression<Func<City, bool>>? filter = null, CancellationToken cancellationToken = default)
        => await _repository.CountAsync(filter, cancellationToken);

    private MemoryCacheEntryOptions BuildEntryOptions()
    {
        var options = new MemoryCacheEntryOptions();
        if (_options.AbsoluteExpirationSeconds is int abs)
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(abs));
        if (_options.SlidingExpirationSeconds is int sliding)
            options.SetSlidingExpiration(TimeSpan.FromSeconds(sliding));
        return options;
    }

    
}
