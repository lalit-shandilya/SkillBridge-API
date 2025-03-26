using Azure.Core;
using MediatR;
using Microsoft.Azure.Cosmos;
using SB.Domain.ValueObjects;
using SB.Infrastructure.Persistence;


namespace SB.Application.Features.Users.Commands;

public class RegisterUserCommand : IRequest<string>
{
    public EmployeeUser UserProfile { get; set; }
}

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
{
    private readonly CosmosDbContext _dbContext;

    public RegisterUserCommandHandler(CosmosDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Ensure the container exists before inserting data
        var container = _dbContext.GetContainer();

        // This checks if the container exists
        // Check if user already exists
        var existingUser = await _dbContext.GetUserByEmailAsync(request.UserProfile.Email);
        if (existingUser != null)
        {
            throw new Exception("User already exists with this email.");
        }

        var regName = request.UserProfile.Name;
        var regAddress = request.UserProfile.Address;
        var userInfo = new SB.Domain.Entities.UserInfo
        {
            Address = regAddress,
            name = regName,
            Email = request.UserProfile.Email,
            Phone = request.UserProfile.PhoneNumber
        };

        // Create new user
        var user = new Domain.Entities.EmployeeUser
        {
            id = Guid.NewGuid().ToString(),
            categoryId = Guid.NewGuid().ToString(),
            isActive = true,
            userProfile = System.Text.Json.JsonSerializer.Serialize(userInfo),
            
            Skills = request.UserProfile.Skills,
            AppliedJobs = request.UserProfile.AppliedJobs,
            Certifications = request.UserProfile.Certifications,
            LinkedInProfileUrl = request.UserProfile.LinkedInProfileUrl,
            ResumeUrl = request.UserProfile.ResumeUrl,
            Role = "Employee" //request.Role.ToString(), hadcoded later will combine with employer
        };
        try
        {
            // Verify container exists before adding
            await container.ReadContainerAsync();

            await container.CreateItemAsync(user, new PartitionKey(user.categoryId));
            await _dbContext.AddUserAsync(user);
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Error: {ex.StatusCode}, SubStatus: {ex.SubStatusCode}, Message: {ex.Message}");
        }
        catch (Exception ex)
        {

            throw;
        }

        return user.id.ToString();
    }
}

