using System.ComponentModel.DataAnnotations;
using ChannelService.Application.Contracts.Validation;

namespace ChannelService.Application.Contracts.Channels;

public class CreateChannelRequest
{
    [Required]
    [NotEmptyGuid]
    public required Guid OwnerUserId { get; set; }

    [Required]
    [RegularExpression(@".*\S.*", ErrorMessage = "Name cannot be empty or whitespace.")]
    [MaxLength(100)]
    public required string Name { get; set; } = string.Empty;

    [MaxLength(400)]
    public string? Description { get; set; }
}