using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SB.Domain.Model;

public class JobSearchModel
{
    [SimpleField(IsKey = true, IsFilterable = true)]
    public string Id { get; set; }

    [SearchableField(IsSortable = true)]
    public string Title { get; set; }

    [SearchableField]
    public string Description { get; set; }

    [SearchableField]
    public string Location { get; set; }

    [SimpleField(IsFilterable = true)]
    public string Company { get; set; }
    [SearchableField]
    [JsonConverter(typeof(SkillsConverter))]
    public List<string> Skills { get; set; }

    [SimpleField(IsFilterable = true, IsSortable = true)]
    public DateTime? PostedDate { get; set; }
}

public class SkillsConverter : JsonConverter<List<string>>
{
    public override List<string> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            return JsonSerializer.Deserialize<List<string>>(ref reader, options);
        }
        else if (reader.TokenType == JsonTokenType.String)
        {
            return new List<string> { reader.GetString() };
        }
        throw new JsonException("Unexpected JSON format for Skills");
    }

    public override void Write(Utf8JsonWriter writer, List<string> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}


