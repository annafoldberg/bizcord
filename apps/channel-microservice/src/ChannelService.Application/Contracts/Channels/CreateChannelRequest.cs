using System.ComponentModel.DataAnnotations;

namespace ChannelService.Application.Contracts.Channels;

public class CreateChannelRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Description { get; set; }
}