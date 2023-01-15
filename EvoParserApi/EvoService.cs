using System.Diagnostics;
using System.Globalization;

internal class EvoService
{
    public EvoService(EvoScraper scraper)
    {
        Scraper = scraper;
    }

    public EvoScraper Scraper { get; }

    public async Task<IEnumerable<EvoProduct>> GetProducts(string request)
    {
        var sw = Stopwatch.StartNew();
        var uri = $"https://www.evo.com/{request}";
        var items = await Scraper.ScrapeAsync(uri);
        foreach (var item in items)
        {
            Console.WriteLine($"{item.RegularPrice}\t{(item.OutletPrice.HasValue ? item.OutletPrice.Value : "--")}\t{(item.DiscountPercent.HasValue ? (item.DiscountPercent.Value).ToString("P") : "--")}\t{item.Name}");
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