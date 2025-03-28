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
                MinExperience = request.MinExperience,
                Skills = request.RequiredSkills,
                IsActive=request.IsActive,
                Location = request.Location,
                Company = request.Company,
                PostedDate=DateTime.Now.Date,
                JobType = request.JobType,
                Salary = request.Salary
            };

            return await _repository.CreateJobPostingAsync(jobPosting);
        }
    }

}
