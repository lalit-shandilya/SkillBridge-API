using MediatR;
using SB.Application.Services.Interface;
using SB.Infrastructure.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SB.Application.Features.Profile.Commands;

    public class UploadResumeHandler : IRequestHandler<UploadResumeCommandRequest, UploadResumeResponse>
    {
        private readonly IBlobStorageService _blobStorageService;

        public UploadResumeHandler(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        public async Task<UploadResumeResponse> Handle(UploadResumeCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                throw new ArgumentException("Invalid file");

        UploadResumeResponse response = new();

            string fileExtension = Path.GetExtension(request.File.FileName);
            string newFileName = $"{Guid.NewGuid()}{fileExtension}";

            using (var stream = request.File.OpenReadStream())
            {
            response.ResumeUrl = await _blobStorageService.UploadFileAsync(stream, newFileName);
            response.ExtractedSkills = ["test1", "test2"];
                return response;
            }
        }
    }


