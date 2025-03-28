using Microsoft.AspNetCore.Http;
using SB.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Infrastructure.Repositories.Interfaces
{
    public interface IJobSearchRepository
    {
        Task<List<JobPosting>> SearchJobsBySkillsAsync(string query);
        Task<List<string>> ExtractSkills(IFormFile File);
    }
}
