using EvoParserApi;
using HtmlAgilityPack;

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
            var uri = new Uri($"https://evo.com{a.GetAttributeValue("href", string.Empty)}");
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
