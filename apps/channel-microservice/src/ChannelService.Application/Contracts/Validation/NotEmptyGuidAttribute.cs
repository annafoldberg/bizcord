using System.ComponentModel.DataAnnotations;

namespace ChannelService.Application.Contracts.Validation;

public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is Guid guid && guid != Guid.Empty;
    }
}