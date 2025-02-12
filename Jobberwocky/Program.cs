using Microsoft.OpenApi.Models;
using Jobberwocky.Models;
using Jobberwocky.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://localhost:5001");
builder.Services.AddControllers();
builder.Services.AddSingleton<JobRepository>();
builder.Services.AddSingleton<IJobService, JobService>();
builder.Services.AddSingleton<SubscriptionService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Job Posting API", Version = "v1" });
});
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Job Posting API v1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();