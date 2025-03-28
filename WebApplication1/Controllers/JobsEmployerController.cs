using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SB.Application.Commands;
using SB.Application.Queries;
using SB.Domain.Model;

namespace SB.API.Controllers
{
    [ApiController]
    [Route("api/Jobs")]
    public class JobsEmployerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobsEmployerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostJob([FromBody] CreateJobPostingCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("GetJobById")]
        public async Task<IActionResult> GetJobById(string jobId)
        {
            var query = new GetJobByIdQuery(jobId);
            var job = await _mediator.Send(query);

            if (job == null)
                return NotFound($"Job with ID {jobId} not found.");

            return Ok(job);
        }


        [HttpGet("jobsbyemployerId/{employerId}")]
        public async Task<IActionResult> GetJobsByEmployer(string employerId)
        {
            if (string.IsNullOrWhiteSpace(employerId))
                return BadRequest("EmployerId is required.");

            var jobs = await _mediator.Send(new GetJobsByEmployerIdQuery(employerId));

            if (jobs == null || jobs.Count == 0)
                return NotFound("No jobs found for this employer.");

            return Ok(jobs);
        }


        [HttpGet("profilesbyskills")]
        public async Task<ActionResult<List<JobPosting>>> SearchJobsBySkills([FromQuery] List<string> skills)
        {
            if (skills == null || skills.Count == 0)
            {
                return BadRequest("At least one skill must be provided.");
            }

            var query = new SearchJobsBySkillsQuery(skills);
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> GetUserProfilesBySkillsAndExperience(
       // [FromQuery] string skills,
       // [FromQuery] int minExperience,
       [FromQuery] string jobid)
        {
            var query = new SearchUserProfilesQuery(jobid);
            var profiles = await _mediator.Send(query);
            return Ok(profiles);
        }

        [HttpPut("{jobId}")]
        public async Task<IActionResult> UpdateJobPosting(string jobId, [FromBody] UpdateJobPostingCommand command)
        {
            if (jobId != command.JobId)
            {
                return BadRequest("Job ID in the URL does not match the request body.");
            }

            var updatedJob = await _mediator.Send(command);
            return Ok(updatedJob);
        }
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetJobById(string id)
        //{
        //    var query = new GetJobPostingByIdQuery(id);
        //    var result = await _mediator.Send(query);
        //    return result != null ? Ok(result) : NotFound();
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetAllJobs()
        //{
        //    var query = new GetAllJobPostingsQuery();
        //    var result = await _mediator.Send(query);
        //    return Ok(result);
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteJob(string id)
        //{
        //    var command = new DeleteJobPostingCommand(id);
        //    await _mediator.Send(command);
        //    return NoContent();
        //}
    }
}





