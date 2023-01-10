using System.Diagnostics;
using System.Globalization;

internal class EvoService
{
    public EvoService(EvoScraper scraper)
    {
        Scraper = scraper;
    }

    public EvoScraper Scraper { get; }

    public async Task<IEnumerable<EvoProduct>> GetProducts()
    {
        var sw = Stopwatch.StartNew();
        var uri = "https://www.evo.com/shop/snowboard/jackets/mens/size_l/size_m/material_gore-tex/s_price-asc/rpp_2";
        var items = await Scraper.ScrapeAsync(uri);
        foreach (var item in items)
        {
            double price = default;
            price = Double.Parse(item.RegularPrice.Substring(1), CultureInfo.InvariantCulture);

            double? outlet = item.OutletPrice == null ? null : Double.Parse(item.OutletPrice.Substring(1), CultureInfo.InvariantCulture);
            double? saving = null;
            if (outlet.HasValue)
            {
                saving = (price - outlet) / price;
            }
            Console.WriteLine($"{price}\t{(outlet.HasValue ? outlet.Value : "--")}\t{(saving.HasValue ? (saving.Value).ToString("P") : "--")}\t{item.Name}");
        }
        sw.Stop();
        Console.WriteLine($"elapsed={sw.Elapsed.TotalMilliseconds}");
        return items;
    }
}