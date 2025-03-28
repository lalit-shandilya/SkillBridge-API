using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SB.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.Application.Queries
{
    public class GetJobsByEmployerIdQueryHandler : IRequestHandler<GetJobsByEmployerIdQuery, List<JobPosting>>
    {
        private readonly Container _container;
        //private readonly Container _profilescontainer;

        public GetJobsByEmployerIdQueryHandler(CosmosClient cosmosClient, IConfiguration configuration)
        {
            var databaseName = configuration["CosmosDb:DatabaseName"];
            var containerName = configuration["CosmosDb:ContainerName"];
            _container = cosmosClient.GetContainer(databaseName, containerName);
            //_profilescontainer = cosmosClient.GetContainer(databaseName, "UserProfiles");
        }

        public async Task<List<JobPosting>> Handle(GetJobsByEmployerIdQuery request, CancellationToken cancellationToken)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.EmployerId = @employerId")
                        .WithParameter("@employerId", request.EmployerId);

            var iterator = _container.GetItemQueryIterator<JobPosting>(query);
            List<JobPosting> jobs = new();

            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                jobs.AddRange(response);
            }

            return jobs;
        }
    }

}
