using System;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;
using SB.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using SB.Domain;

namespace SB.Infrastructure.Services
{

    public class JobSearchService
    {
        private readonly SearchClient _searchClient;

        public JobSearchService(IOptions<AzureCognitiveSearch> settings)
        {
           // var endpoint = new Uri($"https://{settings.Value.ServiceName}.search.windows.net");
            var endpoint = new Uri($"https://{settings.Value.ServiceName}.search.windows.net");
            var credential = new AzureKeyCredential(settings.Value.ApiKey);
           // var credential = new AzureKeyCredential(settings.Value.ApiKey);
            _searchClient = new SearchClient(endpoint, "job-postings-index", credential);
        }


        public async Task<List<JobSearchModel>> SearchJobsAsync(string skill, string location = null, string company = null, int? minExperience = null)
        {
            var filter = BuildFilter(location, company, minExperience); // Build dynamic filter

            var options = new SearchOptions
            {
                IncludeTotalCount = true
            };

            if (!string.IsNullOrEmpty(filter))
            {
                options.Filter = filter; // Only set filter if it's not empty
            }

            var response = await _searchClient.SearchAsync<JobSearchModel>(skill, options);
            List<JobSearchModel> results = new List<JobSearchModel>();

            await foreach (var result in response.Value.GetResultsAsync())
            {
                results.Add(result.Document);
            }

            return results;
        }

        // Function to dynamically build the filter string
        private string BuildFilter(string location, string company, int? minExperience)
        {
            var filters = new List<string>();

            if (!string.IsNullOrEmpty(location))
            {
                filters.Add($"Location eq '{location.Replace("'", "''")}'");  // Escape single quotes
            }

            if (!string.IsNullOrEmpty(company))
            {
                filters.Add($"Company eq '{company.Replace("'", "''")}'");  // Escape single quotes
            }

            if (minExperience.HasValue)
            {
                filters.Add($"MinExperience ge {minExperience.Value}");
            }

            return filters.Count > 0 ? string.Join(" and ", filters) : null;
        }

    }



}
