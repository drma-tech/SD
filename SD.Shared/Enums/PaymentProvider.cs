namespace SD.Shared.Enums
{
    public enum PaymentProvider
    {
        [Custom(Name = "Paddle")]
        Paddle = 1,

        [Custom(Name = "Microsoft Store")]
        Microsoft = 2,

        [Custom(Name = "Google Play")]
        Google = 3,

        [Custom(Name = "App Store")]
        Apple = 4,

        [Custom(Name = "Stripe")]
        Stripe = 5
    }
}