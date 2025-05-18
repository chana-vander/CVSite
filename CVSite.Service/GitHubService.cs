using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Octokit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CVSite.Service
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;
        private readonly GitHubIntegrationOptions _options;

        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            _options = options.Value;

            _client = new GitHubClient(new ProductHeaderValue("CVSite"))
            {
                Credentials = new Credentials(_options.Token)
            };
        }

        public async Task<List<RepositoryPortfolioDto>> GetPortfolioAsync(string userName)
        {
            var repos = await _client.Repository.GetAllForUser(userName);

            var result = new List<RepositoryPortfolioDto>();

            foreach (var repo in repos)
            {
                var pulls = await _client.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name);

                var dto = new RepositoryPortfolioDto
                {
                    Name = repo.Name,
                    Languages = new List<string>(), // אם יש שפה – תוכל להוסיף לפי repo.Id
                    LastCommitDate = repo.UpdatedAt,
                    Stars = repo.StargazersCount,
                    PullRequests = pulls.Count,
                    HtmlUrl = repo.HtmlUrl
                };

                result.Add(dto);
            }

            return result;
        }

        public async Task<List<Repository>> SearchRepositoriesAsync(string? repoName = null, string? language = null, string? userName = null)
        {
            var searchTerms = new List<string>();

            if (!string.IsNullOrWhiteSpace(repoName))
                searchTerms.Add($"{repoName} in:name");

            if (!string.IsNullOrWhiteSpace(userName))
                searchTerms.Add($"user:{userName}");

            if (!string.IsNullOrWhiteSpace(language))
                searchTerms.Add($"language:{language}");

            // אם אין כלל קריטריונים — מחזירים רשימה ריקה בלי לשלוח בקשה מיותרת
            if (searchTerms.Count == 0)
                return new List<Repository>();

            var query = string.Join(" ", searchTerms);

            var request = new SearchRepositoriesRequest(query)
            {
                SortField = RepoSearchSort.Stars,
                Order = SortDirection.Descending
            };

            var result = await _client.Search.SearchRepo(request);

            return result.Items.ToList();
        }



        public async Task<DateTimeOffset?> GetLastUserActivityTime(string userName)
        {
            var events = await _client.Activity.Events.GetAllUserPerformed(userName);
            return events.FirstOrDefault()?.CreatedAt;
        }
    }
}
