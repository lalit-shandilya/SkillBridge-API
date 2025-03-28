using SB.Domain.Model;

namespace SB.Infrastructure.Repositories.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<List<UserProfile>> GetUserProfilesBySkillsAndExperienceAsync(List<string> requiredSkills, int minExperience);
        Task<UserProfile> GetUserProfileByIdAsync(string userId);
    }
}
