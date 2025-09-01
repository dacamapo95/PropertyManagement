using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using PropertyManagement.Domain.Countries;
using PropertyManagement.Infrastructure.Caching;

namespace PropertyManagement.Infrastructure.Database.Repositories;

internal sealed class CachedStateRepository(
    IStateRepository repository, 
    IMemoryCache cache,
    IOptions<CacheOptions> options) : IStateRepository
{
    private readonly IStateRepository _repository = repository;
    private readonly IMemoryCache _cache = cache;
    private readonly EntityCacheOptions _options = options.Value.State;

    public async Task<IReadOnlyList<State>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled) return await _repository.GetByCountryIdAsync(countryId, cancellationToken);
        var key = $"State:ByCountry:{countryId}";
        if (_cache.TryGetValue(key, out IReadOnlyList<State>? states) && states is not null)
            return states;

        states = await _repository.GetByCountryIdAsync(countryId, cancellationToken);
        _cache.Set(key, states, BuildEntryOptions());
        return states;
    }

    public async Task<IReadOnlyList<State>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled) return await _repository.GetAllAsync(cancellationToken);

        var key = "State:All";
        if (_cache.TryGetValue(key, out IReadOnlyList<State>? list))
            return list;

        list = await _repository.GetAllAsync(cancellationToken);
        _cache.Set(key, list, BuildEntryOptions());
        return list;
    }

    public void DisableTracking() => _repository.DisableTracking();
    public async Task<State?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
       => await _repository.GetByIdAsync(id, cancellationToken);

    public async Task<IReadOnlyList<State>> GetAsync(System.Linq.Expressions.Expression<Func<State, bool>> filter, CancellationToken cancellationToken = default)
        => await _repository.GetAsync(filter, cancellationToken);

    public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<State, bool>> filter, CancellationToken cancellationToken = default)
        => await _repository.ExistsAsync(filter, cancellationToken);

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<State, bool>>? filter = null, CancellationToken cancellationToken = default)
        => await _repository.CountAsync(filter, cancellationToken);

    private MemoryCacheEntryOptions BuildEntryOptions()
    {
        var opts = new MemoryCacheEntryOptions();
        if (_options.AbsoluteExpirationSeconds is int abs)
            opts.SetAbsoluteExpiration(TimeSpan.FromSeconds(abs));
        if (_options.SlidingExpirationSeconds is int sliding)
            opts.SetSlidingExpiration(TimeSpan.FromSeconds(sliding));
        return opts;
    }
}
