using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Bson;
using Ardalis.Result;

namespace IMT_Reservas.Server.Application.Features.Archivo;

public class ArchivoService
{
    private readonly IMongoClient _mongoClient;
    private const string DatabaseName = "UCB_Hold";
    private const string BucketName = "archivos";

    public ArchivoService(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    public async Task<Result<string>> Upload(Stream stream, string filename)
    {
        var database = _mongoClient.GetDatabase(DatabaseName);
        var bucket = new GridFSBucket(database, new GridFSBucketOptions { BucketName = BucketName });

        var fileId = await bucket.UploadFromStreamAsync(filename, stream);
        return Result<string>.Success(fileId.ToString());
    }

    public async Task<Result<Stream>> Download(string fileId)
    {
        var database = _mongoClient.GetDatabase(DatabaseName);
        var bucket = new GridFSBucket(database, new GridFSBucketOptions { BucketName = BucketName });

        if (!ObjectId.TryParse(fileId, out var objectId))
            return Result<Stream>.Error("FileId inválido");

        var stream = new MemoryStream();
        await bucket.DownloadToStreamAsync(objectId, stream);
        stream.Position = 0;

        return Result<Stream>.Success(stream);
    }

    public async Task<Result<object>> Delete(string fileId)
    {
        var database = _mongoClient.GetDatabase(DatabaseName);
        var bucket = new GridFSBucket(database, new GridFSBucketOptions { BucketName = BucketName });

        if (!ObjectId.TryParse(fileId, out var objectId))
            return Result<object>.Error("FileId inválido");

        await bucket.DeleteAsync(objectId);
        return Result<object>.Success(new { });
    }
}
