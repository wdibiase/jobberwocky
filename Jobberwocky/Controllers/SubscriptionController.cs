using Jobberwocky.Models;
using Jobberwocky.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jobberwocky.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionController(SubscriptionService subscriptionService) : ControllerBase
{
    private readonly SubscriptionService _subscriptionService = subscriptionService;

    [HttpPost("subscribe")]
    public IActionResult Subscribe([FromBody] Subscription subscription)
    {
        var ok = _subscriptionService.Subscribe(subscription.Email, subscription.Filter);
        if (ok)
        {
            return Ok("Subscription successful");
        }
        else
        {
            return BadRequest("Cannot subscribe");
        }
    }

    [HttpPost("unsubscribe")]
    public IActionResult Unsubscribe([FromBody] Subscription subscription)
    {
        _subscriptionService.Unsubscribe(subscription.Email);
        return Ok("Unsubscription successful");
    }
}

