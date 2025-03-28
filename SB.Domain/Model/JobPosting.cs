using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SB.Domain.Model
{
    public class JobPosting
    {
        [JsonProperty("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [JsonProperty("categoryId")]
        public string CategoryId { get; set; } = Guid.NewGuid().ToString();

        public string EmployerId { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public int MinExperience { get; set; }
        //public List<string> Skills { get; set; }
        public List<string> Skills { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public string SkillsAsString
        {
            get => Skills != null ? System.Text.Json.JsonSerializer.Serialize(Skills) : "[]";
            set => Skills = !string.IsNullOrEmpty(value) ? System.Text.Json.JsonSerializer.Deserialize<List<string>>(value) : new List<string>();
        }
        public string Location { get; set; }
        public string Company { get; set; }
        public DateTime? PostedDate { get; set; } = DateTime.UtcNow;
        public string JobType { get; set; } // Full-time, Part-time, Contract
        public decimal Salary { get; set; }
    }
}
