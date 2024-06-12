using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using MyCollection.Application.Interfaces;

namespace MyCollection.Infrastructure.Services;
public class ImgService() : IImgService
{
    public async ValueTask<string> SaveImgAsync(IFormFile img, Guid pathHelper)
    {

        var blobContainer = new BlobContainerClient("<your connection string>",
            "<your bloc container>");

        var imgPath = $"{pathHelper}{Path.GetExtension(img.FileName)}";

        var blobClient = blobContainer.GetBlobClient(imgPath);

        var memoryStream = new MemoryStream();


        await img.CopyToAsync(memoryStream);

        memoryStream.Position = 0;

        await blobClient.UploadAsync(memoryStream);

        var url = blobClient.Uri.AbsoluteUri;

        return url;
    }

    public async ValueTask DeleteAsync(string path)
    {
        var blobContainer = new BlobContainerClient("<your connection>",
           "<yoiur blob container>");

        var blobName = path.Split("/").LastOrDefault();

        var result = await blobContainer.DeleteBlobIfExistsAsync(blobName);

        Console.WriteLine(result);
    }

    private string GetFullPath(string imgPath, string rootPath) => Path.Combine(rootPath, imgPath);
}
