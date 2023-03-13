using System.Runtime.Serialization;

namespace Headlines.BL.Exceptions
{
    [Serializable]
    public class SerializableException : Exception
    {

        public SerializableException()
        {
        }

        public SerializableException(string message)
            : base(message)
        {
        }

        public SerializableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SerializableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}