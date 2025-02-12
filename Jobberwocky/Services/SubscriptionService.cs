using Jobberwocky.Controllers;
using Jobberwocky.Models;

namespace Jobberwocky.Services;
public class SubscriptionService(ILogger<SubscriptionService> logger)
{
    private readonly List<Subscription> _subscriptions = [];
    private readonly ILogger<SubscriptionService> _logger = logger;

    public bool Subscribe(string email, string? filter = null)
    {
        var response = false;
        if (email is not null)
        {
            _subscriptions.Add(new Subscription { Email = email, Filter = filter });
            response = true;
        }
        return response;
    }

    public int Unsubscribe(string email)
    {
        return _subscriptions.RemoveAll(s => s.Email == email);
    }

    public void NotifySubscriber(Job job)
    {
        foreach (var subscription in _subscriptions)
        {
            if (string.IsNullOrEmpty(subscription.Filter) || job.Title.Contains(subscription.Filter, System.StringComparison.OrdinalIgnoreCase))
            {
                SendEmail(subscription.Email, job);
            }
        }
    }

    private void SendEmail(string email, Job job)
    {
        //SmtpClient smtpClient = new()
        //{
        //    Host = "smtp.gmail.com",
        //    Port = 587,
        //    Credentials = new NetworkCredential("user", "password"),
        //    DeliveryMethod = SmtpDeliveryMethod.Network,
        //    EnableSsl = true
        //};

        var body = $"Se ha agregado un nuevo trabajo: {job.Title}, {job.Description}";

        //try
        //{
        //    smtpClient.Send("empleos@sarasa.com", email, "Nuevo trabajo", body);
        //}
        //catch (SmtpException ex)
        //{
        //    Console.WriteLine(ex.ToString());
        //}

        _logger.LogInformation(body);
    }
}