using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.Domain;
using SB.Domain.Model;
using SB.Infrastructure.Persistence;
using SB.Infrastructure.Repositories.Interfaces;

namespace SB.Infrastructure.Repositories.Implementation
{
  
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly Container _container;

        public UserProfileRepository(IOptions<CosmosDb> settings, CosmosClient cosmosClient)
        {
            var database = cosmosClient.GetDatabase(settings.Value.DatabaseName);
            _container = database.GetContainer("UserProfiles");
        }

        public async Task<Domain.Entities.User> GetUserProfileByIdAsync1(string userId)
        {
            //try
            //{
            //    ItemResponse<Domain.Entities.User> response = await _container.ReadItemAsync<SB.Domain.Entities.User>(userId,new PartitionKey("categortId"));
            //    return response.Resource;
            //}
            //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            //{
            //    return null; // User not found
            //}

            try
            {
                ItemResponse<SB.Domain.Entities.User> response = await _container.ReadItemAsync<SB.Domain.Entities.User>(userId,new PartitionKey("categortId"));
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"CosmosDB Error: {ex.StatusCode}");
                Console.WriteLine($"ActivityId: {ex.ActivityId}");
                Console.WriteLine($"Diagnostics: {ex.Diagnostics}");
            }
            return null;
        }

        public async Task<UserProfile> GetUserProfileByIdAsync(string userId)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                        .WithParameter("@id", userId);

            using FeedIterator<UserProfile> resultSet = _container.GetItemQueryIterator<UserProfile>(query);
            try
            {
                if (resultSet.HasMoreResults)
                {
                    FeedResponse<UserProfile> response = await resultSet.ReadNextAsync();
                    return response.FirstOrDefault();
                } 
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error: {ex.StatusCode}, SubStatus: {ex.SubStatusCode}, Message: {ex.Message}");
            }
           
            return null;
        }

        public async Task<List<UserProfile>> GetUserProfilesBySkillsAndExperienceAsync(List<string> requiredSkills, int minExperience)
        {
            //_logger.LogInformation("Searching for user profiles with skills: {Skills} and minimum experience: {Experience}",
            //                        string.Join(", ", requiredSkills), minExperience);

            var query = new QueryDefinition(@"
            SELECT * FROM c 
            WHERE ARRAY_LENGTH(c.EmployeeProfile.Skills) > 0
            AND EXISTS (
                SELECT VALUE s FROM s IN c.EmployeeProfile.Skills 
                WHERE ARRAY_CONTAINS(@requiredSkills, s.Name)
            )
            AND c.EmployeeProfile.YearsOfExperience >= @minExperience
        ")
            .WithParameter("@requiredSkills", requiredSkills)
            .WithParameter("@minExperience", minExperience);

            var profiles = new List<UserProfile>();
            using FeedIterator<UserProfile> feed = _container.GetItemQueryIterator<UserProfile>(query);

            while (feed.HasMoreResults)
            {
                foreach (var item in await feed.ReadNextAsync())
                {
                    profiles.Add(item);
                }
            }

           // _logger.LogInformation("Found {Count} matching user profiles", profiles.Count);
            return profiles;
        }
    }

}
