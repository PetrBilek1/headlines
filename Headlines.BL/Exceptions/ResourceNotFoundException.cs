using System.Runtime.Serialization;

namespace Headlines.BL.Exceptions
{
    [Serializable]
    public sealed class ResourceNotFoundException : SerializableException
    {
        public ResourceNotFoundException()
        {
        }

        public ResourceNotFoundException(string message)
            : base(message)
        {
        }

        public ResourceNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private ResourceNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}