using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using MediatR;
using SB.Domain.Model;
using SB.Infrastructure.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SB.Application.Queries
{

    public class SearchJobsQueryHandler : IRequestHandler<SearchJobsQuery, List<JobSearchModel>>
    {
        private readonly JobSearchService _searchService;

        public SearchJobsQueryHandler(JobSearchService searchService)
        {
            _searchService = searchService;
        }

        public async Task<List<JobSearchModel>> Handle(SearchJobsQuery request, CancellationToken cancellationToken)
        {
            return await _searchService.SearchJobsAsync(request.SearchbySkill,request.SearchbyLocation,request.SearchbyEmployerName);
        }
    }
}