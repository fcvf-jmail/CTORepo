using WebApi.ProjectStartUp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();

var app = builder.Build();

await app.MigrateDatabaseAsync();

app.UseCustomSwagger();
app.MapControllers();
await app.RunAsync();