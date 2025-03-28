using MediatR;
using SB.Domain.Model;
using SB.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Application.Queries
{
    public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, JobPosting?>
    {
        private readonly IJobRepository _repository;

        public GetJobByIdQueryHandler(IJobRepository repository)
        {
            _repository = repository;
        }

        public async Task<JobPosting?> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetJobByIdAsync(request.JobId);
        }
    }

}
