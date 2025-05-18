using CVSite.Service;
using Microsoft.AspNetCore.Mvc;

namespace CVSite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GitHubController : ControllerBase
    {
        private readonly GitHubService _gitHubService;

        public GitHubController(GitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet("portfolio:{username}")]
        public async Task<IActionResult> GetPortfolio(string username)
        {
            var repos = await _gitHubService.GetPortfolioAsync(username);
            return Ok(repos);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchRepository(string name = "", string language = "", string user = "")
        {
            var repos = await _gitHubService.SearchRepositoriesAsync(name, language, user);

            var results = repos.Select(r => new
            {
                r.Name,
                r.Description,
                r.HtmlUrl,
                r.Language,
                r.UpdatedAt
            });

            return Ok(results);
        }

    }
}
