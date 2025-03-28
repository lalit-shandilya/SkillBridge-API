using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.Domain;
using SB.Domain.Model;
using SB.Infrastructure.Repositories.Interfaces;
using System.Text.Json;

namespace SB.Infrastructure.Repositories.Implementation;

public class JobSearchRepository : IJobSearchRepository
{
    private readonly SearchClient _searchClient;
    private readonly ILogger<JobSearchRepository> _logger;

    public JobSearchRepository(IOptions<AzureCognitiveSearch> settings, ILogger<JobSearchRepository> logger)
    {
        var endpoint = new Uri($"https://{settings.Value.ServiceName}.search.windows.net");
        var credential = new AzureKeyCredential(settings.Value.ApiKey);

        _searchClient = new SearchClient(endpoint, settings.Value.IndexNameJob, credential);
        _logger = logger;
    }

    public async Task<List<JobPosting>> SearchJobsBySkillsAsync(string query)
    {
        _logger.LogInformation("Starting job search for skills: {Query}", query);

        var options = new SearchOptions
        {
            IncludeTotalCount = true,
            QueryType = SearchQueryType.Full,  // Use Full for better text matching
            SearchMode = SearchMode.All,       // Ensures all terms in query must match
            SearchFields = { "Skills" },       // Ensure this field is searchable in Azure Index
            Select = { "id", "Title", "Description", "Skills", "Location", "Company", "Salary" },
            Size = 10
        };

        RunIndexer();
        _logger.LogInformation("Search options: {@Options}", options);

        // Perform search
        var response = await _searchClient.SearchAsync<JobPosting>(query, options);

        var jobs = response.Value.GetResults()
            .Select(r =>
            {
                var job = r.Document;

                // Ensure Skills is correctly deserialized from JSON
                if (job.Skills == null || job.Skills.Count == 0)
                {
                    try
                    {
                        job.Skills = JsonSerializer.Deserialize<List<string>>(r.Document.SkillsAsString) ?? new List<string>();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error deserializing Skills field: {Error}", ex.Message);
                        job.Skills = new List<string>();
                    }
                }

                return job;
            })
            .ToList();

        _logger.LogInformation("Found {Count} jobs matching skills.", jobs.Count);

        return jobs;
    }
    private async Task RunIndexer()
    {
        string searchServiceName = "searchskillservice";
        string indexerName = "skillsearchindexer";
        string apiKey = "W2v9pWw62YtaBu3UVnzZeGqjHwbbULhdlagl1My0UpAzSeACjX15";

        string url = $"https://{searchServiceName}.search.windows.net/indexers/{indexerName}/run?api-version=2023-07-01-Preview";

        using HttpClient client = new HttpClient();
        client.DefaultRequestHeaders.Add("api-key", apiKey);

        HttpResponseMessage response = await client.PostAsync(url, null);

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Indexer triggered successfully.");
        }
        else
        {
            string error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Error triggering indexer: {error}");
        }
    }

}