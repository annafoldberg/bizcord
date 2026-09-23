namespace ChannelService.Domain.Entities;

public class Channel
{
    public int Id { get; set; }
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAt { get; set; }
}