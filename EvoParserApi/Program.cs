var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<EvoScraper>();
builder.Services.AddHttpClient<IRatingService, RatingService>();
builder.Services.AddHttpClient<EvoService>();
var app = builder.Build();

app.MapGet("/gore-tex-jackets", async (EvoService evoService) => await evoService.GetProducts());
app.MapGet("/parse/{*request}", async (EvoService evoService, string request) => await evoService.GetProducts(request));

app.Run();