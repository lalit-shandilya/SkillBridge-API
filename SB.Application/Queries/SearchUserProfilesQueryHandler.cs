using MediatR;
using SB.Domain.Model;
using SB.Infrastructure;
using SB.Infrastructure.Repositories.Interfaces;
using System.Threading.Tasks;

namespace SB.Application.Queries
{
    public class SearchUserProfilesQueryHandler : IRequestHandler<SearchUserProfilesQuery, List<object>>
    {
        private readonly IUserProfileRepository _repository;
        private readonly IJobRepository _jobRepository;

        public SearchUserProfilesQueryHandler(IUserProfileRepository repository,IJobRepository jobRepository)
        {
            _repository = repository;
            _jobRepository = jobRepository;
        }

        public async Task<List<object>> Handle(SearchUserProfilesQuery request, CancellationToken cancellationToken)
        {

            var jobDetail = await _jobRepository.GetJobByIdAsync(request.JobId).ConfigureAwait(false);
            
            var requiredSkills = jobDetail?.Skills;  //.Split(',').Select(s => s.Trim()).ToList();
            
            var userProfiles = await _repository.GetUserProfilesBySkillsAndExperienceAsync(requiredSkills, jobDetail.MinExperience);

            var result = userProfiles
                .Select(user => new
                {
                    user.Name,
                    user.EmployeeProfile.YearsOfExperience,
                    MatchScore = SkillMatcher.CalculateSkillMatchScore(user.EmployeeProfile.Skills.Select(s => s.Name).ToList(), requiredSkills),
                    user.EmployeeProfile.ResumeUrl
                })
                .OrderByDescending(user => user.MatchScore) // Sort by best match
                .ToList<object>(); // Convert to List<object> to match return type

            return result;
        }

        private async Task<int> GetProfileMatchingScore(string jobid, List<UserProfile> userProfiles)
        {
           var jobDeatil = await _jobRepository.GetJobByIdAsync(jobid);
            int noOfSkillsInJobPosting = jobDeatil.Skills.Count;
            int matchScorePercentage = 0;

            foreach (var user in userProfiles)
            {
                var noOfUserSkills = user.EmployeeProfile.Skills.Count + user.EmployeeProfile.ExtractedSkills.Count;
                var ss= (noOfUserSkills / noOfSkillsInJobPosting);
                matchScorePercentage = (int)Math.Round(Convert.ToDouble(noOfUserSkills) / Convert.ToDouble(noOfSkillsInJobPosting) * 100, 0, MidpointRounding.AwayFromZero);
            }
            return matchScorePercentage;
            
        }
    }

}
