using System.Text.Json.Serialization;

internal class ReviewServiceResponse
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("results")]
    public IEnumerable<ReviewResult> Results { get; set; }
}