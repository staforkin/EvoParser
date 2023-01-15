using System.Text.Json.Serialization;

public class ReviewRollup
{
    [JsonPropertyName("average_rating")]
    public double AverageRating { get; set; }

    [JsonPropertyName("rating_count")]
    public int RatingCount { get; set; }

    [JsonPropertyName("review_count")]
    public int ReviewCount { get; set; }

    [JsonPropertyName("answered_questions")]
    public int AnsweredQuestions { get; set; }
}