namespace SD.Shared.Enums
{
    public enum AuthProvider
    {
        [FieldSettings("Firebase")]
        Firebase = 1,

        [FieldSettings("Supabase")]
        Supabase = 2,

        [FieldSettings("Clerk")]
        Clerk = 3,
    }
}