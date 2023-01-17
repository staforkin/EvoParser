using System.Text.Json.Serialization;

internal class ReviewResult
{
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("page_id")]
    public int PageId { get; set; }

    [JsonPropertyName("rollup")]
    public ReviewRollup Rollup { get; set; }
}