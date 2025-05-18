using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVSite.Service
{
    public interface IGitHubService
    {
        Task<List<RepositoryPortfolioDto>> GetPortfolioAsync(string userName);
        Task<List<Repository>> SearchRepositoriesAsync(string? repoName = null, string? language = null, string? userName = null);
    }
}
