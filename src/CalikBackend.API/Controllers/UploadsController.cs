using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalikBackend.API.Controllers;

[ApiController]
[Route("api/uploads")]
[Authorize(Roles = "Admin")]
public class UploadsController : ControllerBase
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".gif"
    };

    private readonly IWebHostEnvironment _env;

    public UploadsController(IWebHostEnvironment env)
    {
        _env = env;
    }

    /// <summary>
    /// Upload one or more images. Returns the URL(s) to store in ImageUrl fields.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Upload(IList<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest(new { message = "No files provided." });

        var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsPath);

        var urls = new List<string>();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { message = $"'{ext}' is not an allowed file type. Allowed: {string.Join(", ", AllowedExtensions)}" });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = $"'{file.FileName}' exceeds the 5 MB size limit." });

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsPath, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            urls.Add($"{baseUrl}/uploads/{fileName}");
        }

        return Ok(new { urls });
    }

    /// <summary>
    /// Delete an uploaded image by filename.
    /// </summary>
    [HttpDelete("{fileName}")]
    public IActionResult Delete(string fileName)
    {
        // Prevent path traversal
        if (fileName.Contains('/') || fileName.Contains('\\') || fileName.Contains(".."))
            return BadRequest(new { message = "Invalid file name." });

        var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound(new { message = "File not found." });

        System.IO.File.Delete(filePath);
        return NoContent();
    }
}
