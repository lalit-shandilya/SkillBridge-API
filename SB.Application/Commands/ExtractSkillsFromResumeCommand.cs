using MediatR;
using Microsoft.AspNetCore.Http;

namespace SB.Application.Commands
{
    public class ExtractSkillsFromResumeCommand : IRequest<List<string>>
    {
        public IFormFile File { get; }

        public ExtractSkillsFromResumeCommand(IFormFile file)
        {
            File = file;
        }
    }
}





