namespace SD.Shared.Models.Subscription;

public class Address
{
    public string? city { get; set; }
    public string? country_code { get; set; }
    public string? description { get; set; }
    public string? first_line { get; set; }
    public string? id { get; set; }
    public string? postal_code { get; set; }
    public string? region { get; set; }
    public string? second_line { get; set; }
    public string? status { get; set; }
}

public class AdjustedTotals
{
    public string? currency_code { get; set; }
    public string? earnings { get; set; }
    public string? fee { get; set; }
    public string? grand_total { get; set; }
    public string? subtotal { get; set; }
    public string? tax { get; set; }
    public string? total { get; set; }
}

public class BillingCycle
{
    public int? frequency { get; set; }
    public string? interval { get; set; }
}

public class BillingPeriod
{
    public DateTime? ends_at { get; set; }
    public DateTime? starts_at { get; set; }
}

public class Card
{
    public int? expiry_month { get; set; }
    public int? expiry_year { get; set; }
    public string? last4 { get; set; }
    public string? type { get; set; }
}

public class Checkout
{
    public string? url { get; set; }
}

public class CustomData
{
    public Features? features { get; set; }
}

public class Customer
{
    public string? email { get; set; }
    public string? id { get; set; }
    public string? locale { get; set; }
    public bool? marketing_consent { get; set; }
    public string? name { get; set; }
    public string? status { get; set; }
}

public class Data
{
    public Address? address { get; set; }
    public string? address_id { get; set; }

    //public object? import_meta { get; set; }
    public BillingCycle? billing_cycle { get; set; }

    public string? business_id { get; set; }
    public DateTime? canceled_at { get; set; }
    public Checkout? checkout { get; set; }

    /// <summary>
    ///     automatic = Payment is collected automatically using a checkout initially, then using a payment method on file.
    ///     manual = Payment is collected manually. Customers are sent an invoice with payment terms and can make a payment
    ///     offline or using a checkout. Requires billing_details.
    /// </summary>
    public string? collection_mode { get; set; }

    public DateTime? created_at { get; set; }
    public string? currency_code { get; set; }

    //public object? billing_details { get; set; }
    public BillingPeriod? current_billing_period { get; set; }

    public CustomData? custom_data { get; set; }
    public Customer? customer { get; set; }
    public string? customer_id { get; set; }
    public Details? details { get; set; }
    public Discount? discount { get; set; }
    public DateTime? first_billed_at { get; set; }
    public string? id { get; set; }

    public List<Item> items { get; set; } = [];

    public ManagementUrls? management_urls { get; set; }

    public DateTime? next_billed_at { get; set; }

    public string? origin { get; set; }

    public DateTime? paused_at { get; set; }

    //public object? scheduled_change { get; set; }
    public List<Payment> payments { get; set; } = [];

    public DateTime? started_at { get; set; }

    /// <summary>
    ///     active = Subscription is active. Paddle is billing for this subscription and related transactions are not past due.
    ///     canceled = Subscription is canceled. Automatically set by Paddle when a scheduled change for a cancellation takes
    ///     effect.
    ///     past_due = Subscription has an overdue payment. Automatically set by Paddle when payment fails for an
    ///     automatically-collected transaction, or payment terms have elapsed for a manually-collected transaction
    ///     paused = Subscription is paused. Automatically set by Paddle when a scheduled change for a pause takes effect.
    ///     trialing = Subscription is in trial.
    /// </summary>
    public string? status { get; set; }

    public string? transaction_id { get; set; }
    public DateTime? updated_at { get; set; }

    public bool IsActive => status == "active";
    public bool IsCanceled => status == "canceled";
}

public class Details
{
    public AdjustedTotals? adjusted_totals { get; set; }

    //public object payout_totals { get; set; }
    public List<LineItem> line_items { get; set; } = [];

    public List<TaxRatesUsed> tax_rates_used { get; set; } = [];
    public Totals? totals { get; set; }
}

public class Discount
{
    public DateTime? ends_at { get; set; }
    public string? id { get; set; }
    public DateTime? starts_at { get; set; }
}

public class Features
{
    public int @enum { get; set; }
}

public class Item
{
    public DateTime? created_at { get; set; }

    /// <summary>
    ///     if null, it means that there will no longer be charges, so you should no longer cancel or update the payment method
    /// </summary>
    public DateTime? next_billed_at { get; set; }

    //public object? trial_dates { get; set; }
    public DateTime? previously_billed_at { get; set; }

    public Price? price { get; set; }

    public Proration? proration { get; set; }
    public int? quantity { get; set; }

    public bool? recurring { get; set; }

    /// <summary>
    ///     active = This item is active. It is not in trial and Paddle bills for it.
    ///     inactive = This item is not active. Set when the related subscription is paused.
    ///     trialing = This item is in trial. Paddle has not billed for it.
    /// </summary>
    public string? status { get; set; }

    public DateTime? updated_at { get; set; }
}

public class LineItem
{
    public string? id { get; set; }
    public string? price_id { get; set; }
    public Product? product { get; set; }
    public Proration? proration { get; set; }
    public int? quantity { get; set; }
    public string? tax_rate { get; set; }
    public Totals? totals { get; set; }
    public UnitTotals? unit_totals { get; set; }
}

public class ManagementUrls
{
    public string? cancel { get; set; }
    public string? update_payment_method { get; set; }
}

public class Meta
{
    public string? request_id { get; set; }
}

public class MethodDetails
{
    public Card? card { get; set; }
    public string? type { get; set; }
}

public class Payment
{
    public string? amount { get; set; }
    public DateTime? captured_at { get; set; }
    public DateTime? created_at { get; set; }
    public string? error_code { get; set; }
    public MethodDetails? method_details { get; set; }
    public string? payment_attempt_id { get; set; }
    public string? status { get; set; }
    public string? stored_payment_method_id { get; set; }
}

public class Price
{
    public BillingCycle? billing_cycle { get; set; }
    public string? description { get; set; }
    public string? id { get; set; }
    public string? product_id { get; set; }

    //public List<object> unit_price_overrides { get; set; } = [];
    public Quantity? quantity { get; set; }

    public string? status { get; set; }
    public string? tax_mode { get; set; }

    //public object? trial_period { get; set; }
    public UnitPrice? unit_price { get; set; }
}

public class Product
{
    public string? description { get; set; }
    public string? id { get; set; }
    public string? image_url { get; set; }
    public string? name { get; set; }
    public string? status { get; set; }
    public string? tax_category { get; set; }
}

public class Proration
{
    public BillingPeriod? billing_period { get; set; }
    public string? rate { get; set; }
}

public class Quantity
{
    public int maximum { get; set; }
    public int minimum { get; set; }
}

public class RootEvent
{
    public Data? data { get; set; }
    public string? event_id { get; set; }
    public string? event_type { get; set; }
    public string? notification_id { get; set; }
    public DateTime? occurred_at { get; set; }
}

public class RootSubscription
{
    public Data? data { get; set; }
    public Meta? meta { get; set; }
}

public class TaxRatesUsed
{
    public string? tax_rate { get; set; }
    public Totals? totals { get; set; }
}

public class Totals
{
    public string? balance { get; set; }

    //public object? fee { get; set; }
    public string? credit { get; set; }

    //public object earnings { get; set; }
    public string? currency_code { get; set; }

    public string? discount { get; set; }
    public string? grand_total { get; set; }
    public string? subtotal { get; set; }
    public string? tax { get; set; }
    public string? total { get; set; }
}

public class UnitPrice
{
    public string? amount { get; set; }
    public string? currency_code { get; set; }
}

public class UnitPriceOverride
{
    public List<string> country_codes { get; set; } = [];
    public UnitPrice? unit_price { get; set; }
}

public class UnitTotals
{
    public string? discount { get; set; }
    public string? subtotal { get; set; }
    public string? tax { get; set; }
    public string? total { get; set; }
}