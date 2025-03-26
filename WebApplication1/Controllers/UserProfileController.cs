using Microsoft.AspNetCore.Mvc;
using SB.Domain.Model;
using SB.Infrastructure.Services;

namespace SB.API.Controllers
{
    [Route("api/userprofiles")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly CosmosDbService _cosmosDbService;

        public UserProfileController(CosmosDbService cosmosDbService)
        {
            _cosmosDbService = cosmosDbService;
        }

        // POST: Create a new user profile
        [HttpPost("CreateProfile")]
        public async Task<IActionResult> CreateUserProfile([FromBody] UserProfile userProfile)
        {
            if (userProfile == null) return BadRequest("Invalid user profile data.");
            if (string.IsNullOrEmpty(userProfile.Id))
            {
                userProfile.Id = Guid.NewGuid().ToString();
            }

            var createdUser = await _cosmosDbService.CreateUserProfileAsync(userProfile);
            return CreatedAtAction(nameof(GetUserProfile), new { email = createdUser.Email }, createdUser);         
        }

        // GET: Retrieve a user profile by email
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserProfile(string email)
        {
            var userProfile = await _cosmosDbService.GetUserProfileAsync(email);
            return userProfile != null ? Ok(userProfile) : NotFound();
        }

        // PUT: Update an existing user profile
        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUserProfile(string email, [FromBody] UserProfile userProfile)
        {
            if (email != userProfile.Email) return BadRequest("Email mismatch.");

            var existingUser = await _cosmosDbService.GetUserProfileAsync(email);
            if (existingUser == null) return NotFound();

            var updatedUser = await _cosmosDbService.UpdateUserProfileAsync(userProfile);
            return Ok(updatedUser);
        }

        // DELETE: Delete a user profile
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUserProfile(string email)
        {
            var existingUser = await _cosmosDbService.GetUserProfileAsync(email);
            if (existingUser == null) return NotFound();

            await _cosmosDbService.DeleteUserProfileAsync(email);
            return NoContent();
        }
    }
}
