namespace PropertyManagement.Application.Options;

public sealed class FileUploadOptions
{
    public string[] AllowedContentTypePrefixes { get; set; } = new[] { "image/" };
    public string[]? AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff" };
    public long? MaxSizeBytes { get; set; }
}
