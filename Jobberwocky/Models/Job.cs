using System.Text.Json.Serialization;

namespace Jobberwocky.Models;
public class Job
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Id { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
}
