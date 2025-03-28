using MediatR;
using SB.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Application.Queries
{
    public class GetJobByIdQuery : IRequest<JobPosting>
    {
        public string JobId { get; }

        public GetJobByIdQuery(string jobId)
        {
            JobId = jobId;
        }
    }

}
