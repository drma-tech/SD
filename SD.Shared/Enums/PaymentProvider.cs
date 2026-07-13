namespace SD.Shared.Enums
{
    public enum PaymentProvider
    {
        [FieldSettings("Paddle")]
        Paddle = 1,

        [FieldSettings("Microsoft Store")]
        Microsoft = 2,

        [FieldSettings("Google Play")]
        Google = 3,

        [FieldSettings("App Store")]
        Apple = 4,

        [FieldSettings("Stripe")]
        Stripe = 5
    }
}
