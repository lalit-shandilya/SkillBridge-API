using Azure;
using Azure.AI.DocumentIntelligence;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace SB.Application.Commands;



public class ExtractSkillsFromResumeHandler : IRequestHandler<ExtractSkillsFromResumeCommand, List<string>>
{
    private readonly IConfiguration _configuration;

    public ExtractSkillsFromResumeHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<string>> Handle(ExtractSkillsFromResumeCommand request, CancellationToken cancellationToken)
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
        await request.File.CopyToAsync(memoryStream, cancellationToken);
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
        var extractedSkills = ExtractSkills(string.Join(" ", extractedText));

        return extractedSkills;
    }

    private List<string> ExtractSkills(string text)
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


