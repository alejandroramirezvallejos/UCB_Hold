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
        try
        {
            var database = _mongoClient.GetDatabase(DatabaseName);
            var bucket = new GridFSBucket(database, new GridFSBucketOptions { BucketName = BucketName });

            var fileId = await bucket.UploadFromStreamAsync(filename, stream);
            return Result<string>.Success(fileId.ToString());
        }
        catch (Exception ex)
        {
            return Result<string>.Error(ex.Message);
        }
    }

    public async Task<Result<Stream>> Download(string fileId)
    {
        try
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
        catch (Exception ex)
        {
            return Result<Stream>.Error(ex.Message);
        }
    }

    public async Task<Result<object>> Delete(string fileId)
    {
        try
        {
            var database = _mongoClient.GetDatabase(DatabaseName);
            var bucket = new GridFSBucket(database, new GridFSBucketOptions { BucketName = BucketName });

            if (!ObjectId.TryParse(fileId, out var objectId))
                return Result<object>.Error("FileId inválido");

            await bucket.DeleteAsync(objectId);
            return Result<object>.Success(new { });
        }
        catch (Exception ex)
        {
            return Result<object>.Error(ex.Message);
        }
    }
}
