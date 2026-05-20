namespace SD.Shared.Models.ZeptoMail
{
    public class Bcc
    {
        public EmailAddress? email_address { get; set; }
    }

    public class Cc
    {
        public EmailAddress? email_address { get; set; }
    }

    public class Detail
    {
        public string? reason { get; set; }
        public string? bounced_recipient { get; set; }
        public DateTime? time { get; set; }
        public string? diagnostic_message { get; set; }
    }

    public class EmailAddress
    {
        public string? address { get; set; }
        public string? name { get; set; }
    }

    public class EmailInfo
    {
        public List<Cc>? cc { get; set; }
        public string? client_reference { get; set; }
        public List<Bcc>? bcc { get; set; }
        public bool? is_smtp_trigger { get; set; }
        public string? subject { get; set; }
        public string? bounce_address { get; set; }
        public bool? is_synced { get; set; }
        public string? email_reference { get; set; }
        public List<ReplyTo>? reply_to { get; set; }
        public From? from { get; set; }
        public List<To>? to { get; set; }
        public string? tag { get; set; }
        public DateTime? processed_time { get; set; }
        public string? @object { get; set; }
    }

    public class EventDatum
    {
        public List<Detail>? details { get; set; }
        public string? @object { get; set; }
    }

    public class EventMessage
    {
        public EmailInfo? email_info { get; set; }
        public List<EventDatum>? event_data { get; set; }
        public string? request_id { get; set; }
    }

    public class From
    {
        public string? address { get; set; }
        public string? name { get; set; }
    }

    public class ReplyTo
    {
        public string? address { get; set; }
        public string? name { get; set; }
    }

    public class ZeptoMailWebHook
    {
        public List<string>? event_name { get; set; }
        public List<EventMessage>? event_message { get; set; }
        public string? mailagent_key { get; set; }
        public string? webhook_request_id { get; set; }
    }

    public class To
    {
        public EmailAddress? email_address { get; set; }
    }
}