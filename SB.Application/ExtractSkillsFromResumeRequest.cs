using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SB.Application
{
   public class ExtractSkillsFromResumeRequest
    {
        [FromForm(Name = "file")]
        public IFormFile File { get; set; }
    }
}
