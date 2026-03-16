namespace Snoopy.Windows.Models;

public class Clip
{
    public required string Name { get; init; }
    public string? StartUrl { get; set; }
    public string? LoopUrl { get; set; }
    public string? EndUrl { get; set; }
    public int Repeat { get; set; }
    public string? From { get; set; }
    public string? To { get; set; }
    public List<string> Others { get; } = new();
}
