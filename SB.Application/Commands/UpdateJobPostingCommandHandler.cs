using MediatR;
using SB.Application.Services.Interface;
using SB.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Application.Commands
{
    public class UpdateJobPostingCommandHandler : IRequestHandler<UpdateJobPostingCommand, JobPosting>
    {
        private readonly IJobPostingRepository _repository;

        public UpdateJobPostingCommandHandler(IJobPostingRepository repository)
        {
            _repository = repository;
        }

        public async Task<JobPosting> Handle(UpdateJobPostingCommand request, CancellationToken cancellationToken)
        {
            // Retrieve existing job posting
            var existingJob = await _repository.GetJobPostingByIdAsync(request.JobId);

            if (existingJob == null || existingJob.EmployerId != request.EmployerId)
            {
                throw new KeyNotFoundException("Item not found for update.");
            }

            // Update fields
            existingJob.Title = request.Title;
            existingJob.Description = request.Description;
            existingJob.MinExperience = request.MinExperience;
            existingJob.Skills = request.RequiredSkills;
            existingJob.Location = request.Location;
            existingJob.Company = request.Company;
            existingJob.IsActive = request.IsActive;
            existingJob.JobType = request.JobType;
            existingJob.Salary = request.Salary;

            // Save changes
            await _repository.UpdateAsync(existingJob);

            return existingJob;
        }
    }

}
