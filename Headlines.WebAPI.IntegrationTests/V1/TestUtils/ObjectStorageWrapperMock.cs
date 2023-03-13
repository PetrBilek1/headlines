using Headlines.BL.Abstractions.ObjectStorageWrapper;
using Headlines.DTO.Entities;

namespace Headlines.WebAPI.Tests.Integration.V1.TestUtils
{
    public sealed class ObjectStorageWrapperMock : IObjectStorageWrapper
    {
        private readonly Dictionary<(string Bucket, string Key), object> _data = new();

        public ObjectStorageWrapperMock(ICollection<(string Bucket, string Key, object Object)> data)
        {
            foreach (var item in data)
            {
                _data.Add((item.Bucket, item.Key), item.Object);
            }
        }

        public Task<T?> DownloadObjectAsync<T>(string bucket, string key, CancellationToken cancellationToken = default) where T : class
        {
            return Task.FromResult((T?) _data[(bucket, key)]);
        }

        public Task<ObjectDataDto> UploadObjectAsync<T>(T data, string bucket, CancellationToken cancellationToken = default) where T : class
        {
            var objectData = new ObjectDataDto
            {
                Bucket = bucket,
                Key = Guid.NewGuid().ToString(),
                ContentType = "application/json",
            };

            _data.Add((objectData.Bucket, objectData.Key), data);

            return Task.FromResult(objectData);
        }
    }
}