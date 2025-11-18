namespace SD.API.Core.Auth
{
    public class FirebaseConfig
    {
        public string type { get; set; } = "service_account";
        public string? project_id { get; set; }
        public string? private_key_id { get; set; }
        public string? private_key { get; set; }
        public string? client_email { get; set; }
        public string? client_id { get; set; }
        public string auth_uri { get; set; } = "https://accounts.google.com/o/oauth2/auth";
        public string token_uri { get; set; } = "https://oauth2.googleapis.com/token";
        public string auth_provider_x509_cert_url { get; set; } = "https://www.googleapis.com/oauth2/v1/certs";
        public string? client_x509_cert_url { get; set; }
        public string universe_domain { get; set; } = "googleapis.com";
    }
}