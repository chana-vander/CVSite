using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CVSite.Service
{
    public class RepositoryPortfolioDto
    {
        public string Name { get; set; }
        public List<string> Languages { get; set; }
        public DateTimeOffset? LastCommitDate { get; set; }
        public int Stars { get; set; }
        public int PullRequests { get; set; }
        public string HtmlUrl { get; set; }
    }
}
