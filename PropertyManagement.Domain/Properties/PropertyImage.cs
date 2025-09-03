using File = PropertyManagement.Domain.Files.File;

namespace PropertyManagement.Domain.Properties;

public sealed class PropertyImage
{
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = default!;

    public Guid FileId { get; set; }
    public File File { get; set; } = default!;
}
