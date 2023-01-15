using System.Text.Json.Serialization;

internal class ReviewResult
{
    [JsonPropertyName("page_id")]
    public string PageId { get; set; }

    [JsonPropertyName("rollup")]
    public ReviewRollup Rollup { get; set; }
}