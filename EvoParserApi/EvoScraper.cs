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
        try
        {
            var a = node.Descendants("a").First();
            var productId = node.GetAttributeValue("data-productid", 0);
            var name = a.Descendants("span").Where(i => i.GetAttributeValue("class", string.Empty).Contains("product-thumb-title")).First().InnerText;
            var uri = new Uri($"https://evo.com{a.GetAttributeValue("href", string.Empty)}");
            var imageUri = new Uri(a.Descendants("img").Where(i => i.GetAttributeValue("class", string.Empty).Contains("product-thumb-image")).First().GetAttributeValue("src", string.Empty));

            string regularPriceString;
            string? outletPriceString = null;
            var outletPriceNode = a.Descendants("span").Where(i => i.GetAttributeValue("class", string.Empty).Contains("discount")).LastOrDefault();
            var productThumbPrice = a.Descendants("span").Where(i => i.HasClass("product-thumb-price"));
            if (outletPriceNode != null)
            {
                if (productThumbPrice.Any(i => i.HasClass("slash")))
                {
                    regularPriceString = productThumbPrice.First(i => i.HasClass("slash")).InnerText;
                }
                else
                {
                    regularPriceString = productThumbPrice.Last().Descendants("span").Last().InnerText;
                }
                outletPriceString = outletPriceNode.InnerText?.Split("\n", StringSplitOptions.RemoveEmptyEntries)?.FirstOrDefault();
            }
            else
            {
                regularPriceString = productThumbPrice.First().Descendants("span").First().InnerText;
            }
            regularPriceString = regularPriceString.Split("\n", StringSplitOptions.RemoveEmptyEntries).First().Trim();
            double price = default;
            price = double.Parse(regularPriceString.Substring(1), CultureInfo.InvariantCulture);
            double? outletPrice = outletPriceString == null ? null : double.Parse(outletPriceString.Substring(1), CultureInfo.InvariantCulture);

            return new EvoProduct(productId, name, uri, imageUri, price, outletPrice, false, null, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(node.InnerHtml);
        }
        return null;
    }
}
