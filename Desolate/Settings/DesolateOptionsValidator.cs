using Microsoft.Extensions.Options;

namespace Desolate.Settings;

public class DesolateOptionsValidator : IValidateOptions<DesolateOptions>
{
    public ValidateOptionsResult Validate(string? name, DesolateOptions options)
    {
        return ValidateOptionsResult.Success;
    }
}