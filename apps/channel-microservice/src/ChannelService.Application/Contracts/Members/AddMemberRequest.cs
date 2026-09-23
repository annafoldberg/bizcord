using System.ComponentModel.DataAnnotations;

namespace ChannelService.Application.Contracts.Members;

public class AddMemberRequest
{
    [Required]
    public Guid UserId { get; set; } // Public UserId
}