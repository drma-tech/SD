using System.Runtime.Serialization;

namespace SD.Shared.Helper
{
    [Serializable]
    public class UnhandledException : Exception
    {
        public UnhandledException() : base()
        {
        }

        public UnhandledException(string? message) : base(message)
        {
        }

        public UnhandledException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UnhandledException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }

    [Serializable]
    public class NotificationException : Exception
    {
        public NotificationException() : base()
        {
        }

        public NotificationException(string? message) : base(message)
        {
        }

        public NotificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected NotificationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}