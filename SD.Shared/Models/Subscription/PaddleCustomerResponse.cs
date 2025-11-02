namespace SD.Shared.Models.Subscription
{
    public class DataCustomer
    {
        public string? id { get; set; }
        public string? status { get; set; }
        public object? custom_data { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public bool marketing_consent { get; set; }
        public string? locale { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public object? import_meta { get; set; }
    }

    //public class Meta
    //{
    //    public string request_id { get; set; }
    //}

    public class PaddleCustomerResponse
    {
        public DataCustomer? data { get; set; }
        public Meta? meta { get; set; }
    }

    public class PaddleCustomerResponseArray
    {
        public DataCustomer[] data { get; set; } = [];
        public Meta? meta { get; set; }
    }
}