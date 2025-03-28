using Azure.Search.Documents;
using MediatR;
using Microsoft.Extensions.Options;
using SB.Domain;
using SB.Domain.Model;
using SB.Infrastructure.Repositories.Interfaces;

namespace SB.Application.Queries;

public class JobRecommendationsHandler : IRequestHandler<JobRecommendationsQuery, List<JobPosting>>
{
    private readonly SearchClient _searchClient;
    private readonly IUserProfileRepository _userProfileRepository;

    public JobRecommendationsHandler(IOptions<AzureCognitiveSearch> settings, IUserProfileRepository userProfileRepository)
    {
        _searchClient = new SearchClient(new Uri(settings.Value.Endpoint), settings.Value.IndexNameJob, new Azure.AzureKeyCredential(settings.Value.ApiKey));
        _userProfileRepository = userProfileRepository;
    }

    public async Task<List<JobPosting>> Handle(JobRecommendationsQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileRepository.GetUserProfileByIdAsync(request.UserId);
        if (userProfile == null) return new List<JobPosting>();
        //String.Join(",", lst)
        string searchQuery = String.Join(" OR ", userProfile.EmployeeProfile.Skills); // Match user skills to jobs
        var options = new SearchOptions { Size = 10 };
        var response = await _searchClient.SearchAsync<JobPosting>(searchQuery, options);

        List<JobPosting> jobs = new List<JobPosting>();
        await foreach (var result in response.Value.GetResultsAsync())
        {
            jobs.Add(result.Document);
        }

        return jobs;        
    }
}

