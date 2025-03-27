using MediatR;
using Microsoft.AspNetCore.Mvc;
using SB.Application;
using SB.Application.Features.Profile.Commands;
using SB.Application.Queries;
using SB.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SB.API.Controllers;


[ApiController]
[Route("jobs")]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<JobSearchModel>>> SearchJobs([FromQuery] string query)
    {
        var result = await _mediator.Send(new SearchJobsQuery(query));
        return Ok(result);
    }

    [HttpGet("searchSkill")]
    public async Task<ActionResult<List<JobPosting>>> SearchJobsBySkills([FromQuery] string query)
    {
        var result = await _mediator.Send(new SearchJobsBySkillsQuery(query));
        return Ok(result);
    }

    [HttpPost("uploadResume")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadResume([FromForm] UploadResumeRequest1 req)
    {
        try
        {
            var uploadResumeResponse = await _mediator.Send(new UploadResumeCommandRequest(req.File));
            return Ok(uploadResumeResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    //[HttpPost("extract-skills")]
    //public async Task<IActionResult> ExtractSkills([FromForm] IFormFile file)
    //{
    //    if (file == null || file.Length == 0)
    //        return BadRequest("No file uploaded");

    //    var result = await _mediator.Send(new ExtractSkillsCommand(file));
    //    return Ok(result);
    //}
}



