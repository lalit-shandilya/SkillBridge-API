using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Application.Features.Profile.Commands;


    public class UploadResumeCommandRequest : IRequest<UploadResumeResponse>
    {
        public IFormFile File { get; }

        public UploadResumeCommandRequest(IFormFile file)
        {
            File = file;
        }
    }


