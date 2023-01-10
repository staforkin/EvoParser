namespace EvoParserApi
{
    interface IScraper<T>
    {
        Task<IEnumerable<T>> ScrapeAsync(string url);
    }
}
