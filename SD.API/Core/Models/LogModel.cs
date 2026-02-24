namespace SD.API.Core.Models
{
    public class LogModel
    {
        public string? Message { get; set; }
        public string? Origin { get; set; } //route or function name
        public string? Params { get; set; } //query parameters or other context info
        public string? Body { get; set; }
        public string? AppVersion { get; set; }
        public string? Ip { get; set; }
    }
}