using System.Reflection.Emit;

namespace SB.Domain.ValueObjects
{
    public record Address(string? Street, string? City, string? State, string? Country, string? Zipcode);
    public record Name(string FirstName, string? LastName);

    public record Skill(string Name, int ProficiencyLevel);

    //public class Address
    //{
    //    public string? Street { get; set; }
    //    public string? City { get; set; }
    //    public string? Country { get; set; }
    //}
    //public class Skill
    //{
    //    public string? Name { get; set; }
    //    public int ProficiencyLevel { get; set; }

    //}
    //public class Name
    //{
    //    public string FirstName { get; set; }
    //    public string? LastName { get; set; }

}


