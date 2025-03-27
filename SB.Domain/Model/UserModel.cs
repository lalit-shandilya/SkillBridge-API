
using Newtonsoft.Json;
using SB.Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace SB.Domain.Model
{
    public class UserProfile
    {
        [System.Text.Json.Serialization.JsonIgnore]
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString(); // CosmosDB requires an ID
        [JsonProperty("email")]
        public string Email { get; set; } // Partition Key 
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public EmployeeProfile EmployeeProfile { get; set; } = new();
        public EmployerProfile EmployerProfile { get; set; } = new();
    }

    //public record Address(string? Street, string? City, string? State, string? Country);
    //public record NameModel(string FirstName, string? LastName);
    //public record Skill(string Name, int ProficiencyLevel);

    public class EmployeeProfile
    {
        public List<Skill> Skills { get; set; } = new();
        public List<Skill> ExtractedSkills { get; set; } = new();
        public bool UserConsentforSkills { get; set; }
        
        public List<string> Certifications { get; set; } = new();
        public List<string> AppliedJobs { get; set; } = new();
        public string ResumeUrl { get; set; }
        public string LinkedInProfileUrl { get; set; }
        public int YearsOfExperience { get; set; }
    }

    public class EmployerProfile
    {
        public string CompanyName { get; set; }
        public string CompanyWebsiteUrl { get; set; }
        public IList<CompanyLocation> CompanyLocations { get; set; } = new List<CompanyLocation>();
    }

    public record CompanyLocation(string Address, string City, string State, string Country, string? Zipcode);

}
