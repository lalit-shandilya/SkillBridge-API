using MediatR;
using SB.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Application.Commands
{
    public class UpdateJobPostingCommand : IRequest<JobPosting>
    {
        public string JobId { get; set; }
        public string EmployerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int MinExperience { get; set; }
        public List<string> RequiredSkills { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public bool IsActive { get; set; }
        public string JobType { get; set; }
        public decimal Salary { get; set; }
    }

}
