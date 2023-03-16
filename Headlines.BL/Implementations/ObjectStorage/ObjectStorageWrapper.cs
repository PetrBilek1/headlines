using Headlines.BL.Abstractions.ObjectStorage;
using Headlines.DTO.Attributes;
using Headlines.DTO.Entities;
using Newtonsoft.Json;
using PBilek.ObjectStorageService;
using System.Text;

namespace Headlines.BL.Implementations.ObjectStorage
{
    public sealed class ObjectStorageWrapper : IObjectStorageWrapper
    {
        private const string JsonContentType = "application/json";

        private readonly IObjectStorageService _objectStorageService;

        public ObjectStorageWrapper(IObjectStorageService objectStorageService) 
        { 
            _objectStorageService = objectStorageService;
        }

        public async Task<ObjectDataDto> UploadObjectAsync<T>(T data, string bucket, CancellationToken cancellationToken = default)
            where T : class
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));

            var nameAttribute = Attribute.GetCustomAttribute(typeof(T), typeof(ObjectStorageNameAttribute)) as ObjectStorageNameAttribute;
            var key = $"{nameAttribute?.Name ?? nameof(T)}/{Guid.NewGuid()}.json";

            await _objectStorageService.PutObjectAsync(bucket, key, JsonContentType, stream, cancellationToken);

            return new ObjectDataDto
            {
                Bucket = bucket,
                Key = key,
                ContentType = JsonContentType
            };
        }

        public async Task<T?> DownloadObjectAsync<T>(string bucket, string key, CancellationToken cancellationToken = default)
            where T : class
        {
            using var stream = await _objectStorageService.DownloadObjectAsync(bucket, key, cancellationToken);
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            var bytes = memoryStream.ToArray();

            var objectContent = Encoding.UTF8.GetString(bytes);

            return JsonConvert.DeserializeObject<T>(objectContent);
        }
    }
}