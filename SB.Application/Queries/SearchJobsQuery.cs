using MediatR;
using SB.Domain.Model;
using System.Collections.Generic;

namespace SB.Application.Queries
{
    public class SearchJobsQuery : IRequest<List<JobSearchModel>>
    {
        public string SearchbySkill { get; set; }
        public string? SearchbyLocation { get; set; }
        public string? SearchbyEmployerName { get; set; }

        public SearchJobsQuery(string searchbySkill, string? searchbyLocation =null, string? searchbyEmployerName = null)
        {
            SearchbySkill = searchbySkill;
            SearchbyLocation = searchbyLocation;
            SearchbyEmployerName = searchbyEmployerName;
        }
    }

}
