using MediatR;
using SB.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Application.Queries
{
    public class SearchUserProfilesQuery : IRequest<List<object>>
    {
       // public string Skills { get; }
        public string JobId { get; }
       // public int MinExperience { get; }

        public SearchUserProfilesQuery(string jobId)
        {
          //  Skills = skills;
          //  MinExperience = minExperience;
            JobId = jobId;
        }
    }

}
