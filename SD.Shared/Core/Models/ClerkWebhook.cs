namespace SD.Shared.Core.Models
{
    public class Data
    {
        public bool deleted { get; set; }
        public string? id { get; set; }
        //public string? @object { get; set; }
    }

    public class EventAttributes
    {
        public HttpRequest? http_request { get; set; }
    }

    public class HttpRequest
    {
        public string? client_ip { get; set; }
        public string? user_agent { get; set; }
    }

    public class ClerkWebhook
    {
        public Data? data { get; set; }
        public EventAttributes? event_attributes { get; set; }
        //public string? @object { get; set; }
        public long timestamp { get; set; }
        public string? type { get; set; }
    }
}