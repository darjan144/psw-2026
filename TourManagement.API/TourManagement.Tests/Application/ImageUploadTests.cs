using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TourManagement.API.Controllers;

namespace TourManagement.Tests.Application;

public class ImageUploadTests
{
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly string _tempDir;

    public ImageUploadTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.WebRootPath).Returns(_tempDir);
    }

    [Fact]
    public async Task Upload_ValidImage_ReturnsUrl()
    {
        var controller = new ImageController(_envMock.Object);
        var file = CreateFakeFile("test.jpg", "image/jpeg", 100);

        var result = await controller.Upload(file, CancellationToken.None);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var urlProp = ok.Value!.GetType().GetProperty("url");
        urlProp.Should().NotBeNull();
        var url = urlProp!.GetValue(ok.Value) as string;
        url.Should().StartWith("/uploads/");
        url.Should().EndWith(".jpg");

        var savedPath = Path.Combine(_tempDir, "uploads", Path.GetFileName(url!));
        File.Exists(savedPath).Should().BeTrue();
    }

    [Fact]
    public async Task Upload_NoFile_ReturnsBadRequest()
    {
        var controller = new ImageController(_envMock.Object);

        var result = await controller.Upload(null!, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Upload_EmptyFile_ReturnsBadRequest()
    {
        var controller = new ImageController(_envMock.Object);
        var file = CreateFakeFile("empty.jpg", "image/jpeg", 0);

        var result = await controller.Upload(file, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Upload_NonImageFile_ReturnsBadRequest()
    {
        var controller = new ImageController(_envMock.Object);
        var file = CreateFakeFile("script.exe", "application/octet-stream", 100);

        var result = await controller.Upload(file, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Upload_TooLargeFile_ReturnsBadRequest()
    {
        var controller = new ImageController(_envMock.Object);
        var file = CreateFakeFile("huge.jpg", "image/jpeg", 6 * 1024 * 1024);

        var result = await controller.Upload(file, CancellationToken.None);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    private static IFormFile CreateFakeFile(string fileName, string contentType, int sizeBytes)
    {
        var stream = new MemoryStream(new byte[sizeBytes]);
        return new FormFile(stream, 0, sizeBytes, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}
