using FluentValidation;
using Microsoft.Extensions.Options;
using PropertyManagement.Application.Options;

namespace PropertyManagement.Application.Features.Files.Create;

public sealed class CreateFileCommandValidator : AbstractValidator<CreateFileCommand>
{
    public CreateFileCommandValidator(IOptions<FileUploadOptions> options)
    {
        var opt = options.Value;

        RuleFor(x => x.FileName)
            .NotEmpty();

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(ct => opt.AllowedContentTypePrefixes == null || opt.AllowedContentTypePrefixes.Length == 0 ||
                         opt.AllowedContentTypePrefixes.Any(p => ct.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            .WithMessage(x => $"ContentType '{x.ContentType}' not allowed.");

        RuleFor(x => x.Data)
            .NotNull()
            .Must(data => data is { Length: > 0 })
            .WithMessage("Empty file.");

        RuleFor(x => x)
            .Must(x =>
            {
                if (opt.AllowedExtensions is null || opt.AllowedExtensions.Length == 0) return true;
                var ext = Path.GetExtension(x.FileName);
                return opt.AllowedExtensions.Any(e => string.Equals(e, ext, StringComparison.OrdinalIgnoreCase));
            })
            .WithMessage(x => $"Extension '{Path.GetExtension(x.FileName)}' not allowed.");

        RuleFor(x => x.Data)
            .Must(data =>
            {
                if (data is null) return false;
                if (!opt.MaxSizeBytes.HasValue) return true;
                return data.LongLength <= opt.MaxSizeBytes.Value;
            })
            .WithMessage(_ => "File too large.");
    }
}
