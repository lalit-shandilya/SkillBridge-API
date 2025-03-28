using Azure.AI.DocumentIntelligence;
using Azure;
using MediatR;
using SB.Application.Commands;
using SB.Application.Services.Interface;
using SB.Infrastructure.Repositories.Interfaces;
using SB.Infrastructure.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SB.Application.Features.Profile.Commands;

    public class UploadResumeHandler : IRequestHandler<UploadResumeCommandRequest, UploadResumeResponse>
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly IJobSearchRepository _jobSearchRepository;
    private readonly IConfiguration _configuration;

    public UploadResumeHandler(IBlobStorageService blobStorageService, IJobSearchRepository jobSearchRepository, IConfiguration configuration)
        {
            _blobStorageService = blobStorageService;
        _jobSearchRepository = jobSearchRepository;
        _configuration = configuration;

        }

        public async Task<UploadResumeResponse> Handle(UploadResumeCommandRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                throw new ArgumentException("Invalid file");

        UploadResumeResponse response = new();
        var skills= await ExtractSkills(request).ConfigureAwait(false);

            string fileExtension = Path.GetExtension(request.File.FileName);
            string newFileName = $"{Guid.NewGuid()}{fileExtension}";

            using (var stream = request.File.OpenReadStream())
            {
            response.ResumeUrl = await _blobStorageService.UploadFileAsync(stream, newFileName);
            response.ExtractedSkills = skills;
                return response;
            }
        }

    public async Task<List<string>> ExtractSkills(UploadResumeCommandRequest request)
    {
        var azureEndpoint = _configuration["DocumentIntelligence:Endpoint"];
        var apiKey = _configuration["DocumentIntelligence:ApiKey"];
        var modelId = "prebuilt-read"; // Use built-in "prebuilt-read" model for text extraction

        if (string.IsNullOrEmpty(azureEndpoint) || string.IsNullOrEmpty(apiKey))
        {
            throw new Exception("Azure Document Intelligence API credentials are missing.");
        }

        var credential = new AzureKeyCredential(apiKey);
        var client = new DocumentIntelligenceClient(new Uri(azureEndpoint), credential);

        // Convert file to BinaryData
        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);
        BinaryData fileData = BinaryData.FromBytes(memoryStream.ToArray());

        // ✅ Call Azure AI to analyze the document
        Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(
            WaitUntil.Completed,
            modelId,
            fileData
        );

        AnalyzeResult result = operation.Value;

        if (result == null || string.IsNullOrEmpty(result.Content))
        {
            throw new Exception("Failed to extract text from the document.");
        }

        // ✅ Extract text from the document
        List<string> extractedText = new List<string> { result.Content };

        // ✅ Extract skills from text
        var extractedSkills = GetSkills(string.Join(" ", extractedText));

        return extractedSkills;
    }

    private List<string> GetSkills(string text)
    {
        var knownSkills = new HashSet<string> { "Installation", "Repair", "Welding", "Soldering","Plumbing", "Leak Detection","woodwork", "framing","finishing","drawings","operating hand tools",
            "operating power tools","Knowledge of safety protocols","Expertise in cooking techniques","Inventory management","Handling customer feedback","Grilling","Grilling & frying techniques","Knife handling & chopping",
            "C#", "ASP.NET", "Azure", "AI", "SQL", "Python", "Java", "JavaScript", "Machine Learning", "DevOps" };
        var extractedSkills = new List<string>();

        foreach (var skill in knownSkills)
        {
            if (text.IndexOf(skill, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                extractedSkills.Add(skill);
            }
        }

        return extractedSkills;
    }
}


