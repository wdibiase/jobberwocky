namespace Jobberwocky.Services;

using Jobberwocky.Models;
using System.Collections.Generic;
using System.Linq;

public interface IJobService
{
    List<Job> GetAllJobs(string storageType);
    Job AddJob(Job job, string storageType);
    List<Job> SearchJobs(string title);
}

public class JobService(JobRepository repository, SubscriptionService subscriptionService) : IJobService
{
    private readonly JobRepository _repository = repository;
    private readonly List<Job> _jobsInMemory = [];
    private readonly SubscriptionService _subscriptionService = subscriptionService;
    private int _nextId = 1;

    public List<Job> GetAllJobs(string storageType)
    {
        return storageType.Equals("db", StringComparison.CurrentCultureIgnoreCase) ? _repository.GetAllJobs() : _jobsInMemory;
    }

    public Job AddJob(Job job, string storageType)
    {
        if (storageType.Equals("db", StringComparison.CurrentCultureIgnoreCase))
        {
            _repository.AddJob(job);
        }
        else
        {
            job.Id = _nextId++;
            _jobsInMemory.Add(job);
        }

        _subscriptionService.NotifySubscriber(job);
        return job;
    }

    public List<Job> SearchJobs(string title) =>
        _jobsInMemory.Where(j => j.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
}

