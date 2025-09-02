using PropertyManagement.Shared.Primitives;

namespace PropertyManagement.Domain.Files;

public class File : AuditableEntity<Guid>
{
    public string Name { get; set; } = default!;

    public string OriginalName { get; set; } = default!;
        
    public string MimeType { get; set; } = default!;
        
    public string Extension { get; set; } = default!;

    public long Size { get; set; }

    public string? Url { get; set; }

    public byte[]? Content { get; set; }
}
