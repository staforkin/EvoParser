using System.Text.Json;

internal class RatingService : IRatingService
{
    //https://display.powerreviews.com/m/4163/l/en_US/product/203890/snippet?apikey=e5fcb978-8192-44d7-8fd1-b4e14fd1a523&_noconfig=true
    private const string ApiKey = "e5fcb978-8192-44d7-8fd1-b4e14fd1a523";
    private readonly HttpClient _httpClient;
    public RatingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ReviewServiceResponse?> GetRatingsAsync(int[] ids)
    {
        var uri = $"https://display.powerreviews.com/m/4163/l/en_US/product/{ids[0]}/snippet?apikey={ApiKey}&_noconfig=true";
        ReviewServiceResponse? result = null;
        
        try
        {
            var response = await _httpClient.GetStringAsync(uri);
            result = JsonSerializer.Deserialize<ReviewServiceResponse>(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return result;
    }
}
