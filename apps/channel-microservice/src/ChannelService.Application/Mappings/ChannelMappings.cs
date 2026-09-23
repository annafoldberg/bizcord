using ChannelService.Application.Contracts.Channels;
using ChannelService.Domain.Entities;

namespace ChannelService.Application.Mappings;

public static class ChannelMappings
{
    public static ChannelResponse ToResponse(this Channel channel)
    {
        return new ChannelResponse
        {
            Id = channel.PublicId,
            Name = channel.Name,
            Description = channel.Description,
            CreatedAt = channel.CreatedAt,
            LastUpdatedAt = channel.LastUpdatedAt
        };
    }

    public static Channel ToEntity(this CreateChannelRequest request)
    {
        return new Channel
        {
            Name = request.Name,
            Description = request.Description
        };
    }
}