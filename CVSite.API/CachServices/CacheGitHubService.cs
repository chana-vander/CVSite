using Microsoft.Extensions.Caching.Memory;
using Octokit;
using CVSite.Service;
namespace CVSite.API.CachServices
{
    public class CacheGithubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private readonly GitHubClient _gitHubClient;

        private record CacheEntry(List<RepositoryPortfolioDto> Data, DateTimeOffset LastChecked);

        public CacheGithubService(IGitHubService gitHubService, IMemoryCache memoryCach, GitHubClient gitHubClient)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCach;
            _gitHubClient = gitHubClient;
        }

        public async Task<List<RepositoryPortfolioDto>> GetPortfolioAsync(string userName)
        {
            var cacheKey = $"portfolio_{userName}";
            if (_memoryCache.TryGetValue(cacheKey, out CacheEntry cacheEntry))
            {
                var lastCacheTime = cacheEntry.LastChecked;
                var latestEventTime = await GetLastUserActivityTime(userName);

                if (latestEventTime <= lastCacheTime)
                {
                    return cacheEntry.Data;
                }
                else
                {
                    _memoryCache.Remove(cacheKey);
                }
            }
            var portfolio = await _gitHubService.GetPortfolioAsync(userName);
            var now = DateTimeOffset.UtcNow;

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            _memoryCache.Set(cacheKey, new CacheEntry(portfolio, now), cacheOptions);

            return portfolio;
        }

        public Task<List<Repository>> SearchRepositoriesAsync(string? repoName = null, string? language = null, string? userName = null)
        {
            return _gitHubService.SearchRepositoriesAsync(repoName, language, userName);
        }

        public async Task<DateTimeOffset?> GetLastUserActivityTime(string userName)
        {
            var events=await _gitHubClient.Activity.Events.GetAllUserPerformed(userName);
            return events.FirstOrDefault()?.CreatedAt??DateTimeOffset.MinValue;
        }
    }
}
