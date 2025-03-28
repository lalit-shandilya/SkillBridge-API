using SB.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Infrastructure.Repositories.Interfaces
{
    public interface IJobRepository
    {
        Task<JobPosting?> GetJobByIdAsync(string jobId);
    }

}
