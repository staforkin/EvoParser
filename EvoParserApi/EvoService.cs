using System.Diagnostics;

internal class EvoService
{
    private readonly HttpClient httpClient;
    private readonly IRatingService ratingService;

    public EvoService(EvoScraper scraper, HttpClient httpClient, IRatingService ratingService)
    {
        Scraper = scraper;
        this.httpClient = httpClient;
        this.ratingService = ratingService;
    }

    public EvoScraper Scraper { get; }

    public async Task<IEnumerable<EvoProduct>> GetProducts(string request)
    {
        var sw = Stopwatch.StartNew();
        var uri = $"https://www.evo.com/{request}";
        var items = await Scraper.ScrapeAsync(httpClient, uri);
        var tasks = new List<Task>();
        foreach (var item in items)
        {
            var ratingTask = ratingService.GetRatingsAsync(new int[] { item.Id });
            tasks.Add(ratingTask);
        }
        await Task.WhenAll(tasks);
        foreach (var item in items)
        {
            var rating = (Task<ReviewServiceResponse>)tasks.FirstOrDefault(task => ((Task<ReviewServiceResponse>)task).Result.Results.FirstOrDefault()?.PageId == item.Id);
            item.Rating = rating?.Result != null ? rating.Result.Results.FirstOrDefault()?.Rollup.AverageRating : null;
            item.ReviewsCount = rating?.Result != null ? rating.Result.Results.FirstOrDefault()?.Rollup.ReviewCount : null;

            Console.WriteLine($"{item.RegularPrice}\t{(item.OutletPrice.HasValue ? item.OutletPrice.Value : "--")}" +
                $"\t{(item.DiscountPercent.HasValue ? (item.DiscountPercent.Value).ToString("P") : "--")}" +
                $"\t{(item.Rating.HasValue ? item.Rating : "--")}" +
                $"\t{(item.ReviewsCount.HasValue ? item.ReviewsCount : "--")}" +
                $"\t{item.Name}");
        }
        sw.Stop();
        Console.WriteLine($"elapsed={sw.Elapsed.TotalMilliseconds}");
        return items;
    }

    public async Task<IEnumerable<EvoProduct>> GetProducts()
    {
        return await GetProducts("/shop/snowboard/jackets/mens/size_l/size_m/material_gore-tex/s_price-asc/rpp_2");
    }
}