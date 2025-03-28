using MediatR;
using Microsoft.AspNetCore.Mvc;
using SB.Application;
using SB.Application.Commands;
using SB.Application.Features.Profile.Commands;
using SB.Application.Queries;
using SB.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SB.API.Controllers;


[ApiController]
[Route("api")]
public class JobsEmployeeController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsEmployeeController(IMediator mediator)
    {
        _mediator = mediator;
    }



    [HttpGet("search")]
    public async Task<ActionResult<List<JobSearchModel>>> SearchJobs( string searchbySkill, string? searchbyLocation, string? searchbyEmployerName)
    {
        var result = await _mediator.Send(new SearchJobsQuery(searchbySkill,searchbyLocation , searchbyEmployerName ));
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

    [HttpPost("extract-skills")]
    public async Task<IActionResult> ExtractSkills(ExtractSkillsFromResumeRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("Invalid file.");
        }

        var skills = await _mediator.Send(new ExtractSkillsFromResumeCommand(request.File));

        return Ok(new { skills });
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



