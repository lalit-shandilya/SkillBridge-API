using MediatR;
using Microsoft.Extensions.Logging;
using SB.Domain.Model;
using SB.Infrastructure.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SB.Application.Queries
{   

    public class SearchJobsBySkillsQueryHandler : IRequestHandler<SearchJobsBySkillsQuery, List<JobPosting>>
    {
        private readonly IJobSearchRepository _jobSearchRepository;
        private readonly ILogger<SearchJobsBySkillsQueryHandler> _logger;

        public SearchJobsBySkillsQueryHandler(IJobSearchRepository jobSearchRepository, ILogger<SearchJobsBySkillsQueryHandler> logger)
        {
            _jobSearchRepository = jobSearchRepository;
            _logger = logger;
        }

        public async Task<List<JobPosting>> Handle(SearchJobsBySkillsQuery request, CancellationToken cancellationToken)
        {
            string formattedQuery = string.Join(" OR ", request.Skills); // Convert list into OR query
            return await _jobSearchRepository.SearchJobsBySkillsAsync(formattedQuery);
        }
    }

}
