namespace Jobberwocky.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Jobberwocky.Services;
using Jobberwocky.Models;

[ApiController]
[Route("api/jobs")]
public class JobController(IJobService jobService, IHttpClientFactory httpClientFactory) : ControllerBase
{
    [HttpGet]
    public IActionResult GetJobs([FromQuery] string storage = "ram" )
    {
        var jobs = jobService.GetAllJobs(storage);
        return Ok(jobs);
    }

    [HttpPost]
    public IActionResult CreateJob([FromBody] Job job, [FromQuery] string storage = "ram")
    {
        if (job == null || string.IsNullOrWhiteSpace(job.Title))
            return BadRequest("Invalid job data.");

        var createdJob = jobService.AddJob(job, storage);
        return CreatedAtAction(nameof(GetJobs), new { id = createdJob.Id }, createdJob);
    }

    [HttpGet("search")]
    public IActionResult SearchJobs([FromQuery] string title)
    {
        var results = jobService.SearchJobs(title);
        return Ok(results);
    }

    [HttpGet("extra")]
    public async Task<IActionResult> GetCombinedJobs([FromQuery] string storageType)
    {
        var localJobs = jobService.GetAllJobs(storageType);
        var externalJobs = await FetchExternalJobs();
        foreach (var job in externalJobs)
        {
            jobService.AddJob(job, storageType);
        }

        return Ok(localJobs.Concat(externalJobs));
    }

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();
    private async Task<List<Job>> FetchExternalJobs()
    {
        var jobs = new List<Job>();
        var response = await _httpClient.GetAsync("http://localhost:8080/jobs");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var externalData = JsonSerializer.Deserialize<Dictionary<string, List<List<JsonElement>>>>(json);
            int externalIdCounter = 10000;

            if (externalData == null || externalData.Count == 0)
            {
                return jobs;
            }

            foreach (var country in externalData)
            {
                if (country.Value == null) continue; // Avoids a NullReferenceException

                foreach (var jobData in country.Value)
                {
                    if (jobData is null) continue;

                    string? title = jobData[0].GetString();
                    int? salary = jobData[1].GetInt32();
                    string? skills = jobData[2].GetString();

                    jobs.Add(new Job
                    {
                        Id = externalIdCounter++,
                        Title = title,
                        Description = $"Salary: {salary} - Skills: {skills}"
                    });
                }
            }
        }

        return jobs;
    }
}
