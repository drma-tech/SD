using System.Runtime.Serialization;

namespace SD.Shared.Helper
{
    [Serializable]
    public class NotificationException : Exception
    {
        public NotificationException() : base()
        {
        }

        public NotificationException(string message) : base(message)
        {
        }

        public NotificationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NotificationException(HttpResponseMessage response) : base(response?.Content.ReadAsStringAsync().Result)
        {
        }

        protected NotificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}