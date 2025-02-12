namespace Jobberwocky.Models;

using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class JobRepository(IConfiguration configuration)
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

    public List<Job> GetAllJobs()
    {
        var jobs = new List<Job>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("SELECT Id, Title, Description FROM Jobs", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            jobs.Add(new Job
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2)
            });
        }
        return jobs;
    }

    public void AddJob(Job job)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();
        using var command = new SqlCommand("INSERT INTO Jobs (Title, Description) VALUES (@title, @description)", connection);
        command.Parameters.AddWithValue("@title", job.Title);
        command.Parameters.AddWithValue("@description", job.Description);
        command.ExecuteNonQuery();
    }
}
