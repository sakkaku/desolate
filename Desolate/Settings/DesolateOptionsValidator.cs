using Microsoft.Extensions.Options;

namespace Desolate.Settings;

/// <summary>
///     Validates the options for the engine.
/// </summary>
public class DesolateOptionsValidator : IValidateOptions<DesolateOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, DesolateOptions options)
    {
        if (options.Editor.Enabled && !options.Web.Enabled)
            return ValidateOptionsResult.Fail("Editor requires web to be enabled.");

        return ValidateOptionsResult.Success;
    }
}