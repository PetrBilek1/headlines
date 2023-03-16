using Headlines.DTO.Entities;

namespace Headlines.BL.Abstractions.ObjectStorage
{
    public interface IObjectStorageWrapper
    {
        Task<ObjectDataDto> UploadObjectAsync<T>(T data, string bucket, CancellationToken cancellationToken = default)
            where T : class;
        Task<T?> DownloadObjectAsync<T>(string bucket, string key, CancellationToken cancellationToken = default)
            where T : class;
    }
}