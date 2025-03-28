using MediatR;
using SB.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Application.Queries
{
    public class GetJobsByEmployerIdQuery : IRequest<List<JobPosting>>
    {
        public string EmployerId { get; }

        public GetJobsByEmployerIdQuery(string employerId)
        {
            EmployerId = employerId;
        }
    }
}



