namespace SD.Shared.Enums
{
    public enum AuthProvider
    {
        [Custom(Name = "Firebase")]
        Firebase = 1,

        [Custom(Name = "Supabase")]
        Supabase = 2,
    }
}