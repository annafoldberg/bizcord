using System.ComponentModel.DataAnnotations;

namespace ChannelService.Application.Contracts.Channels;

public class UpdateChannelRequest
{
    [RegularExpression(@".*\S.*", ErrorMessage = "Name cannot be empty or whitespace.")]
    [MaxLength(100)]
    public string? Name { get; set; }

    [MaxLength(400)]
    public string? Description { get; set; }
}