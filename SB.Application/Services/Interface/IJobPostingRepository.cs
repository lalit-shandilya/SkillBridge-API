using SB.Domain.Model;

namespace SB.Application.Services.Interface
{
    public interface IJobPostingRepository
    {
        Task<JobPosting> CreateJobPostingAsync(JobPosting job);
        Task<JobPosting> GetJobPostingByIdAsync(string jobId);
        Task<List<JobPosting>> GetAllJobPostingsAsync();
        Task DeleteJobPostingAsync(string jobId);
        Task UpdateAsync(JobPosting entity);
    }
}

