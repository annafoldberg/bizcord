using System.ComponentModel.DataAnnotations;
using ChannelService.Domain.Enums;

namespace ChannelService.Application.Contracts.Members;

public class UpdateMemberRoleRequest
{
    [Required]
    public MemberRole Role { get; set; }
}