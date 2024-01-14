namespace SD.Shared.Models.Subscription
{
    public class BillingCycle
    {
        public string? interval { get; set; }
        public int frequency { get; set; }
    }

    public class CurrentBillingPeriod
    {
        public DateTime? ends_at { get; set; }
        public DateTime? starts_at { get; set; }
    }

    public class Discount
    {
        public string? id { get; set; }
        public DateTime? ends_at { get; set; }
        public DateTime? starts_at { get; set; }
    }

    public class SubscriptionPaddle
    {
        public string? event_id { get; set; }
        public string? event_type { get; set; }
        public DateTime? occurred_at { get; set; }
        public string? notification_id { get; set; }
        public Data? data { get; set; }
    }

    public class Data
    {
        public string? id { get; set; }
        public List<Item> items { get; set; } = [];
        /// <summary>
        /// active = Subscription is active. Paddle is billing for this subscription and related transactions are not past due.
        /// canceled = Subscription is canceled. Automatically set by Paddle when a scheduled change for a cancelation takes effect.
        /// past_due = Subscription has an overdue payment. Automatically set by Paddle when payment fails for an automatically-collected transaction, or payment terms have elapsed for a manually-collected transaction
        /// paused = Subscription is paused. Automatically set by Paddle when a scheduled change for a pause takes effect.
        /// trialing = Subscription is in trial.
        /// </summary>
        public string? status { get; set; }
        public DateTime? paused_at { get; set; }
        public string? address_id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? started_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string? business_id { get; set; }
        public DateTime? canceled_at { get; set; }
        public Discount? discount { get; set; }
        public CustomData? custom_data { get; set; }
        public string? customer_id { get; set; }
        public BillingCycle? billing_cycle { get; set; }
        public string? currency_code { get; set; }
        public DateTime? next_billed_at { get; set; }
        public string? transaction_id { get; set; }
        //public object? billing_details { get; set; }
        /// <summary>
        /// automatic = Payment is collected automatically using a checkout initially, then using a payment method on file.
        /// manual = Payment is collected manually. Customers are sent an invoice with payment terms and can make a payment offline or using a checkout. Requires billing_details.
        /// </summary>
        public string? collection_mode { get; set; }
        public DateTime? first_billed_at { get; set; }
        //public object? scheduled_change { get; set; }
        public CurrentBillingPeriod? current_billing_period { get; set; }
        //public object? import_meta { get; set; }
    }

    public class CustomData
    {
        public Features? features { get; set; }
    }

    public class Features
    {
        public int @enum { get; set; }
    }

    public class Item
    {
        public Price? price { get; set; }
        /// <summary>
        /// active = This item is active. It is not in trial and Paddle bills for it.
        /// inactive = This item is not active. Set when the related subscription is paused.
        /// trialing = This item is in trial. Paddle has not billed for it.
        /// </summary>
        public string? status { get; set; }
        public int quantity { get; set; }
        public bool recurring { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        //public object trial_dates { get; set; }
        public DateTime? next_billed_at { get; set; }
        public DateTime? previously_billed_at { get; set; }
    }

    public class Price
    {
        public string? id { get; set; }
        public string? tax_mode { get; set; }
        public string? product_id { get; set; }
        public UnitPrice? unit_price { get; set; }
        public string? description { get; set; }
        public BillingCycle? billing_cycle { get; set; }
    }

    public class UnitPrice
    {
        public string? amount { get; set; }
        public string? currency_code { get; set; }
    }
}