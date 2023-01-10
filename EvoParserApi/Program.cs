var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<EvoScraper>();
builder.Services.AddScoped<EvoService>();
var app = builder.Build();

app.MapGet("/gore-tex-jackets", async (EvoService evoService) => await evoService.GetProducts());

app.Run();