using Microsoft.Extensions.Options;
using Octokit;

namespace CVSite.Service
{
    public class GitHubService : IGitHubService
    {
        private readonly GitHubClient _client;

        public GitHubService(IOptions<GitHubSettings> options)
        {
            var settings = options.Value;

            _client = new GitHubClient(new ProductHeaderValue("CVSite"))
            {
                Credentials = new Credentials(settings.Token)
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
                    Languages = new List<string>(), 
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
            var request = new SearchRepositoriesRequest(repoName ?? "")
            {
                User = userName
            };

            if (!string.IsNullOrEmpty(language) && Enum.TryParse<Language>(language, true, out var parsedLanguage))
            {
                request.Language = parsedLanguage;
            }

            var result = await _client.Search.SearchRepo(request);
            return result.Items.ToList();
        }
    }

}
