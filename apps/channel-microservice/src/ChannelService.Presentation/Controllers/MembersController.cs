namespace ChannelService.Presentation.Controllers;

[ApiController]
[Route("/channels/{channelId}/[controller]")]
public sealed class MembersController : ControllerBase
{
    private readonly IMemberService _service;

    public MembersController(IMemberService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> AddMember(
        [FromRoute] Guid channelId,
        [FromBody] AddMemberRequest request,
        CancellationToken ct)
    {
        var result = await _service.AddMemberAsync(channelId, request, ct);
        
        return result.Type switch
        {
            AddMemberResultType.ChannelNotFound => NotFound("Channel was not found."),
            AddMemberResultType.AlreadyMember => Conflict("User is already a member of this channel."),
            AddMemberResultType.Success => CreatedAtAction(
                nameof(GetMember),
                new { channelId, userId = result.Member!.UserId },
                result.Member),
            _ => Problem()
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetChannelMembers([FromRoute] Guid channelId, CancellationToken ct)
    {
        var result = await _service.GetChannelMembersAsync(channelId, ct);

        return result.Type switch
        {
            GetChannelMembersResultType.ChannelNotFound => NotFound("Channel was not found."),
            GetChannelMembersResultType.Success => Ok(result.Members),
            _ => Problem()
        };
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetMember(
        [FromRoute] Guid channelId,
        [FromRoute] Guid userId,
        CancellationToken ct)
    {
        var result = await _service.GetMemberAsync(channelId, userId, ct);

        return result.Type switch
        {
            GetMemberResultType.ChannelNotFound => NotFound("Channel was not found."),
            GetMemberResultType.MemberNotFound => NotFound("Member was not found."),
            GetMemberResultType.Success => Ok(result.Member),
            _ => Problem()
        };
    }

    [HttpPatch("{userId}")]
    public async Task<IActionResult> UpdateMemberRole(
        [FromRoute] Guid channelId,
        [FromRoute] Guid userId,
        [FromBody] UpdateMemberRoleRequest request,
        CancellationToken ct)
    {
        var result = await _service.UpdateMemberRoleAsync(channelId, userId, request, ct);

        return result switch
        {
            UpdateMemberRoleResult.ChannelNotFound => NotFound("Channel was not found."),
            UpdateMemberRoleResult.MemberNotFound => NotFound("Member was not found."),
            UpdateMemberRoleResult.CannotDemoteOnlyOwner => Conflict("Cannot demote only channel owner."),
            UpdateMemberRoleResult.Success => NoContent(),
            _ => Problem()
        };
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> RemoveMember(
        [FromRoute] Guid channelId,
        [FromRoute] Guid userId,
        CancellationToken ct)
    {
        var result = await _service.RemoveMemberAsync(channelId, userId, ct);

        return result switch
        {
            RemoveMemberResult.ChannelNotFound => NotFound("Channel was not found."),
            RemoveMemberResult.MemberNotFound => NotFound("Member was not found."),
            RemoveMemberResult.CannotRemoveOnlyOwner => Conflict("Cannot remove only channel owner."),
            RemoveMemberResult.Success => NoContent(),
            _ => Problem()
        };
    }
}