namespace SD.API.Core.Models
{
    public class InApp
    {
        public string? quantity { get; set; }
        public string? product_id { get; set; }
        public string? transaction_id { get; set; }
        public string? original_transaction_id { get; set; }
        public string? purchase_date { get; set; }
        public string? purchase_date_ms { get; set; }
        public string? purchase_date_pst { get; set; }
        public string? original_purchase_date { get; set; }
        public string? original_purchase_date_ms { get; set; }
        public string? original_purchase_date_pst { get; set; }
        public string? is_trial_period { get; set; }
        public string? in_app_ownership_type { get; set; }
    }

    public class Receipt
    {
        public string? receipt_type { get; set; }
        public long adam_id { get; set; }
        public long app_item_id { get; set; }
        public string? bundle_id { get; set; }
        public string? application_version { get; set; }
        public long download_id { get; set; }
        public long version_external_identifier { get; set; }
        public string? receipt_creation_date { get; set; }
        public string? receipt_creation_date_ms { get; set; }
        public string? receipt_creation_date_pst { get; set; }
        public string? request_date { get; set; }
        public string? request_date_ms { get; set; }
        public string? request_date_pst { get; set; }
        public string? original_purchase_date { get; set; }
        public string? original_purchase_date_ms { get; set; }
        public string? original_purchase_date_pst { get; set; }
        public string? original_application_version { get; set; }
        public List<InApp> in_app { get; set; } = [];
    }

    public class LatestReceiptInfo
    {
        public string? quantity { get; set; }
        public string? product_id { get; set; }
        public string? transaction_id { get; set; }
        public string? original_transaction_id { get; set; }
        public string? purchase_date { get; set; }
        public string? purchase_date_ms { get; set; }
        public string? purchase_date_pst { get; set; }
        public string? original_purchase_date { get; set; }
        public string? original_purchase_date_ms { get; set; }
        public string? original_purchase_date_pst { get; set; }
        public string? expires_date { get; set; }
        public string? expires_date_ms { get; set; }
        public string? expires_date_pst { get; set; }
        public string? cancellation_date { get; set; }
        public string? cancellation_date_ms { get; set; }
        public string? cancellation_date_pst { get; set; }
        public string? is_in_intro_offer_period { get; set; }
        public string? is_trial_period { get; set; }
        public string? is_upgraded { get; set; }
        public string? auto_renew_status { get; set; }
        public string? is_in_billing_retry_period { get; set; }
        public string? web_order_line_item_id { get; set; }
        public string? subscription_group_identifier { get; set; }
        public string? promotion_type { get; set; }
        public string? promotional_offer_id { get; set; }
    }

    public class AppleResponseReceipt
    {
        public Receipt? receipt { get; set; }
        public List<LatestReceiptInfo> latest_receipt_info { get; set; } = [];
        public string? environment { get; set; }
        public int status { get; set; }
    }
}