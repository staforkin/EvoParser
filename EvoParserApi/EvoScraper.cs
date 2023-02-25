using EvoParserApi;
using HtmlAgilityPack;
using System.Globalization;

internal class EvoScraper : IScraper<EvoProduct>
{
    public async Task<IEnumerable<EvoProduct>> ScrapeAsync(HttpClient httpClient, string url)
    {
        var res = new List<EvoProduct>();
        var response = await httpClient.GetStringAsync(url);
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
            var product = BuildProductFrom(node);
            if (product != null)
            {
                res.Add(product);
            }
        }

        return res;
    }

    private EvoProduct? BuildProductFrom(HtmlNode node)
    {
        string regularPriceString = string.Empty;
        string? outletPriceString = null;
        try
        {
            var a = node.Descendants("a").First();
            var productId = node.GetAttributeValue("data-productid", 0);
            var name = a.Descendants("span").Where(i => i.GetAttributeValue("class", string.Empty).Contains("product-thumb-title")).First().InnerText;
            var uri = new Uri($"https://evo.com{a.GetAttributeValue("href", string.Empty)}");
            var imageUri = new Uri(a.Descendants("img").Where(i => i.GetAttributeValue("class", string.Empty).Contains("product-thumb-image")).First().GetAttributeValue("src", string.Empty));

            bool clearance = false;
            var outletPriceNodes = a.Descendants("span").Where(i => i.GetAttributeValue("class", string.Empty).Contains("discount")).ToList();
            var productThumbPrice = a.Descendants("span").Where(i => i.HasClass("product-thumb-price"));
            if (outletPriceNodes.Any())
            {
                if (productThumbPrice.Any(i => i.HasClass("slash")))
                {
                    regularPriceString = productThumbPrice.First(i => i.HasClass("slash")).InnerText;
                }
                else
                {
                    regularPriceString = productThumbPrice.Last().Descendants("span").Last().InnerText;
                }
                var outletCount = outletPriceNodes.Count;
                // 0    1   2   3
                // outlet: xxx - yyy sale
                // outlet: xxx
                // xxx - yyy sale
                var outletNode = outletCount % 2 == 0 ? outletPriceNodes[1] : outletPriceNodes[0];
                outletPriceString = outletNode.InnerText?.Split("\n", StringSplitOptions.RemoveEmptyEntries)?.FirstOrDefault();
                var clearanceNode = outletPriceNodes.LastOrDefault()?.Descendants("span").Where(i => i.HasClass("product-thumb-sale")).FirstOrDefault();
                if (clearanceNode != null)
                {
                    clearance = clearanceNode.InnerText.Equals("Clearance", StringComparison.OrdinalIgnoreCase);
                }
            }
            else
            {
                regularPriceString = productThumbPrice.First().Descendants("span").First().InnerText;
            }
            regularPriceString = regularPriceString.Split("\n", StringSplitOptions.RemoveEmptyEntries).First().Trim();
            regularPriceString = regularPriceString.Split("-", StringSplitOptions.RemoveEmptyEntries).First().Trim();
            double price = default;
            price = double.Parse(regularPriceString.Substring(1), CultureInfo.InvariantCulture);
            double? outletPrice = outletPriceString == null ? null : double.Parse(outletPriceString.Substring(1), CultureInfo.InvariantCulture);

            return new EvoProduct(productId, name, uri, imageUri, price, outletPrice, false, null, null, clearance);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"{regularPriceString?.ToString()}\t{outletPriceString?.ToString()}");
        }
        return null;
    }
}
