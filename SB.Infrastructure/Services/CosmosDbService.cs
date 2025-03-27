using Microsoft.Azure.Cosmos;
using SB.Domain.Model;

namespace SB.Infrastructure.Services
{

    public class CosmosDbService
    {
        private readonly Container _container;

        public CosmosDbService(string endpoint, string key, string databaseId, string containerId)
        {
            var cosmosClient = new CosmosClient(endpoint, key);
            _container = cosmosClient.GetDatabase(databaseId).GetContainer("UserProfiles");
        }

        // Create User Profile
        public async Task<UserProfile> CreateUserProfileAsync(UserProfile userProfile)
        {
            return await _container.CreateItemAsync(userProfile, new PartitionKey(userProfile.Email));
        }

        // Get User Profile by Email
        public async Task<UserProfile?> GetUserProfileAsync(string email)
        {
            try
            {
                //var response = await _container.ReadItemAsync<UserProfile>(id, new PartitionKey(email));
                //return response.Resource;

                var query = new QueryDefinition("SELECT * FROM c WHERE c.email = @email")
                  .WithParameter("@email", email);

                var iterator = _container.GetItemQueryIterator<UserProfile>(query);
                var results = new List<UserProfile>();

                while (iterator.HasMoreResults)
                {
                    foreach (var item in await iterator.ReadNextAsync())
                    {
                        results.Add(item);
                    }
                }
                return results.FirstOrDefault();
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            } 
        }

        // Update User Profile
        public async Task<UserProfile> UpdateUserProfileAsync(UserProfile userProfile)
        {
            return await _container.UpsertItemAsync(userProfile, new PartitionKey(userProfile.Email));
        }

        // Delete User Profile
        public async Task DeleteUserProfileAsync(string email)
        {
            await _container.DeleteItemAsync<UserProfile>(email, new PartitionKey(email));
        }
    }

}
