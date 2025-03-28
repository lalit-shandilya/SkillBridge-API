using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.Domain;
using SB.Domain.Model;
using SB.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Infrastructure.Repositories.Implementation
{
    public class JobRepository : IJobRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;
        private readonly ILogger<JobRepository> _logger;

        public JobRepository(IOptions<CosmosDb> settings, ILogger<JobRepository> logger)
        {
            _cosmosClient = new CosmosClient(settings.Value.AccountEndpoint, settings.Value.AccountKey);
            _container = _cosmosClient.GetContainer(settings.Value.DatabaseName, "SB_Container");
            _logger = logger;
        }

        public async Task<JobPosting?> GetJobByIdAsync(string jobId)
        {
            try
            {
                var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @jobId")
                    .WithParameter("@jobId", jobId);

                using FeedIterator<JobPosting> feed = _container.GetItemQueryIterator<JobPosting>(query);

                while (feed.HasMoreResults)
                {
                    foreach (var item in await feed.ReadNextAsync())
                    {
                        return item; // Returning the first match
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching job by ID {JobId}: {Error}", jobId, ex.Message);
                return null;
            }
        }
    }

}
