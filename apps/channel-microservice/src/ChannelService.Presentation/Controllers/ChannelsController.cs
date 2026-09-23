namespace ChannelService.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ChannelsController : ControllerBase
{
    private readonly IChannelService _service;

    public ChannelsController(IChannelService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> CreateChannel([FromBody] CreateChannelRequest request, CancellationToken ct)
    {
        var channel = await _service.CreateChannelAsync(request, ct);
        
        return CreatedAtAction(
            nameof(GetChannel),
            new { id = channel.Id },
            channel);
    }

    [HttpGet]
    public async Task<IActionResult> GetChannels(CancellationToken ct)
    {
        var channels = _service.GetChannelsAsync(ct);
        
        return Ok(channels);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetChannel([FromRoute] Guid id, CancellationToken ct)
    {
        var channel = await _service.GetChannelAsync(id, ct);

        if (channel is null) return NotFound();

        return Ok(channel);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateChannel([FromRoute] Guid id, [FromBody] UpdateChannelRequest request, CancellationToken ct)
    {
        var updated = await _service.UpdateChannelAsync(id, request, ct);

        if (!updated) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChannel([FromRoute] Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteChannelAsync(id, ct);

        if (!deleted) return NotFound();

        return NoContent();
    }
}