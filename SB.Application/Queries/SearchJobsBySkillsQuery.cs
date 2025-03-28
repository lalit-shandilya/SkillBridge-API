using MediatR;
using SB.Domain.Model;
using System.Collections.Generic;


namespace SB.Application.Queries
{
    
    public class SearchJobsBySkillsQuery : IRequest<List<JobPosting>>
    {
        public List<string> Skills { get; set; }

        public SearchJobsBySkillsQuery(List<string> skills)
        {
            Skills = skills ?? new List<string>();
        }
    }

}
