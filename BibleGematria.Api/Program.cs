using BibleGematria.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string dataDir = Path.Combine(AppContext.BaseDirectory, "Data");
builder.Services.AddSingleton(new TanachRepository(dataDir));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();