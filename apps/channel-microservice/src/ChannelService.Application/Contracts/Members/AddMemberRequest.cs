using System.ComponentModel.DataAnnotations;
using ChannelService.Application.Contracts.Validation;

namespace ChannelService.Application.Contracts.Members;

public class AddMemberRequest
{
    [Required]
    [NotEmptyGuid]
    public Guid UserId { get; set; } // Public UserId
}