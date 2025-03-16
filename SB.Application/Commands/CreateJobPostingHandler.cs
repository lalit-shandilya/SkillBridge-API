﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SB.Application.Services.Interface;
using SB.Domain.Model;

namespace SB.Application.Commands
{  

    public class CreateJobPostingHandler : IRequestHandler<CreateJobPostingCommand, JobPosting>
    {
        private readonly IJobPostingRepository _repository;

        public CreateJobPostingHandler(IJobPostingRepository repository)
        {
            _repository = repository;
        }

        public async Task<JobPosting> Handle(CreateJobPostingCommand request, CancellationToken cancellationToken)
        {
            var jobPosting = new JobPosting
            {
                EmployerId = request.EmployerId,
                Title = request.Title,
                Description = request.Description,
                Skills = request.RequiredSkills.First(),
                Location = request.Location,
                JobType = request.JobType,
                Salary = request.Salary
            };

            return await _repository.CreateJobPostingAsync(jobPosting);
        }
    }

}
