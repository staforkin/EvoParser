namespace EvoParserApi
{
    interface IScraper<T>
    {
        Task<IEnumerable<T>> ScrapeAsync(HttpClient httpClient, string url);
    }
}
