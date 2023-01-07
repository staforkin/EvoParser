using System.Diagnostics;
using System.Globalization;
using HtmlAgilityPack;

namespace EvoParser;

internal class Program
{
    // https://www.evo.com/shop/snowboard/jackets/mens/size_l/size_m/material_gore-tex/s_price-asc/rpp_2
    static async Task Main(string[] args)
    {
        var sw = Stopwatch.StartNew();
        var uri = "https://www.evo.com/shop/snowboard/jackets/mens/size_l/size_m/material_gore-tex/s_price-asc/rpp_10";
        var scraper = new EvoScraper();
        var items = await scraper.ScrapeAsync(uri);
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
    }
}

interface IScraper<T>
{
    Task<IEnumerable<T>> ScrapeAsync(string url);
}

internal class EvoScraper : IScraper<EvoProduct>
{
    public async Task<IEnumerable<EvoProduct>> ScrapeAsync(string url)
    {
        var res = new List<EvoProduct>();
        HttpClient client = new HttpClient();
        var response = await client.GetStringAsync(url);
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(response);
        if (htmlDocument == null)
        {
            throw new ArgumentException("Failed to load XDocument from stream");
        }
        var productNodes = htmlDocument.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "").Contains("js-product-thumb"))
                    .ToList();

        foreach (var node in productNodes)
        {
            var a = node.Descendants("a").First();
            var productId = node.GetAttributeValue("data-productid", 0);
            var name = a.Descendants("span").Where(i => i.GetAttributeValue("class", string.Empty).Contains("product-thumb-title")).First().InnerText;
            var uri = new Uri(a.GetAttributeValue("href", string.Empty));
            var imageUri = new Uri(a.Descendants("img").Where(i => i.GetAttributeValue("class", string.Empty).Contains("product-thumb-image")).First().GetAttributeValue("src", string.Empty));

            string regularPrice;
            string? outletPrice = null;
            var outletPriceNode = a.Descendants("span").Where(i => i.GetAttributeValue("class", string.Empty).Contains("discount")).LastOrDefault();
            if (outletPriceNode != null)
            {
                regularPrice = a.Descendants("span").Where(i => i.GetAttributeValue("class", string.Empty).Contains("product-thumb-price slash")).First().InnerText;
                outletPrice = outletPriceNode.InnerText?.Split("\n", StringSplitOptions.RemoveEmptyEntries)?.FirstOrDefault();
            }
            else
            {
                regularPrice = a.Descendants("span").Where(i => i.GetAttributeValue("class", string.Empty).Contains("product-thumb-price")).First()
                            .Descendants("span").First().InnerText;
            }
            regularPrice = regularPrice.Split("\n", StringSplitOptions.RemoveEmptyEntries).First();
            res.Add(new EvoProduct(productId, name, uri, imageUri, regularPrice, outletPrice, false, null, null));
        }

        return res;
    }
}

internal class EvoProduct
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public Uri? Uri { get; private set; }
    public Uri? ImageUri { get; private set; }
    public string RegularPrice { get; private set; }
    public string? OutletPrice { get; private set; }
    public bool InStoreOnly { get; private set; }
    public Double? Rating { get; private set; }
    public int? ReviewsCount { get; private set; }
    public EvoProduct(int id, string name, Uri uri, Uri imageUri, string regularPrice, string? outletPrice, bool inStoreOnly, double? rating, int? reviewsCount)
    {
        Id = id;
        Name = name;
        Uri = uri;
        ImageUri = imageUri;
        RegularPrice = regularPrice;
        OutletPrice = outletPrice;
        InStoreOnly = inStoreOnly;
        Rating = rating;
        ReviewsCount = reviewsCount;
    }
}