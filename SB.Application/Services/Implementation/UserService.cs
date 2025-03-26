using Microsoft.Azure.Cosmos;
using SB.Application.Services.Interface;

namespace SB.Application.Services.Implementation
{
    public class UserService<T> : IUserService<T> where T : class
    {

        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public UserService(CosmosClient cosmosClient, string databaseName, string containerName)
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(databaseName, containerName);
        }
        //private readonly IUserRepository<EmployeeUser> _userRepository;

        //public async Task DeleteUserAsync(string id)
        //{
        //    await _userRepository.DeleteAsync(id);
        //}

        public async Task<IEnumerable<T>> GetAllUsersAsync()
        {
            var query = _container.GetItemQueryIterator<T>("SELECT * FROM c");
            List<T> results = new List<T>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange(response);
            }
            return results;
        }

        public async Task<T> GetUserByIdAsync(string id, string partitionKey)
        {
            try
            {
                //ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                //return response.Resource;

                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                T user = response.Resource;

                // If T is User, deserialize `UserProfile`
                //if (user is User userEntity)
                //{
                //    userEntity.DeserializeUserProfile();
                //}

                return user;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default; // Return default value for T (null for reference types)
            }
        }

        public async Task AddUserAsync(T userDto)
        {
            await _container.CreateItemAsync(userDto);
        }

        public async Task UpdateUserAsync(T userDto)
        {
            dynamic user = userDto;
            string id = user.id;  // Ensure your model has an "id" property
            await _container.UpsertItemAsync(userDto, new PartitionKey(id));
        }

        public async Task DeleteUserAsync(string id)
        {
            await _container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }

        //Task<IEnumerable<EmployeeUser>> IUserService<EmployeeUser>.GetAllUsersAsync()
        //{
        //    var users = await _userRepository.GetAllAsync();
        //    return users.Select(u => new EmployeeUser
        //    {
        //        Id = u.id,
        //        FirstName = u.firstName,
        //        LastName = u.lastName,
        //        Email = u.email,
        //        Role = u.role.ToString(),
        //        IsActive = u.isActive
        //    });
        //}

        //Task<EmployeeUser> IUserService<EmployeeUser>.GetUserByIdAsync(string id)
        //{
        //    var user = await _userRepository.GetByIdAsync(id);
        //    return user != null ? new UserDto
        //    {
        //        Id = user.id,
        //        FirstName = user.firstName,
        //        LastName = user.lastName,
        //        Email = user.email,
        //        Role = user.role.ToString(),
        //        IsActive = user.isActive
        //    } : null;
        //}

        //public Task AddUserAsync(EmployeeUser userDto)
        //{
        //    var user = new User
        //    {
        //        id = userDto.Id,
        //        firstName = userDto.FirstName,
        //        lastName = userDto.LastName,
        //        email = userDto.Email,
        //        role = userDto.Role,
        //        isActive = userDto.IsActive
        //    };
        //    await _userRepository.AddAsync(user);
        //}

        //public Task UpdateUserAsync(EmployeeUser userDto)
        //{
        //    var user = new User
        //    {
        //        id = userDto.Id,
        //        firstName = userDto.FirstName,
        //        lastName = userDto.LastName,
        //        email = userDto.Email,
        //        role = userDto.Role,
        //        isActive = userDto.IsActive
        //    };
        //    await _userRepository.UpdateAsync(user);
        //}
    }

}
