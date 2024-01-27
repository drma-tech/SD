namespace SD.Shared.Helper
{
    public static class SubscriptionHelper
    {
        private static Restrictions GetRestrictions(this AccountProduct? product)
        {
            return product switch
            {
                AccountProduct.Basic => new BasicRestrictions(),
                AccountProduct.Standard => new StandardRestrictions(),
                AccountProduct.Premium => new PremiumRestrictions(),
                _ => new BasicRestrictions(),
            };
        }

        public static void ValidateFavoriteProviders(AccountProduct? product, int qtd)
        {
            var restriction = GetRestrictions(product);

            if (qtd > restriction.FavoriteProviders)
            {
                throw new NotificationException("Your current plan does not support this operation");
            }
        }

        public static void ValidateWishList(AccountProduct? product, int qtd)
        {
            var restriction = GetRestrictions(product);

            if (qtd > restriction.MyWishlist)
            {
                throw new NotificationException("Your current plan does not support this operation");
            }
        }
    }

    public abstract class Restrictions
    {
        public abstract int FavoriteProviders { get; }
        public abstract int MySuggestions { get; }
        public abstract int MyWishlist { get; }
        public abstract int MyWatching { get; }
    }

    public class BasicRestrictions : Restrictions
    {
        public override int FavoriteProviders => 1;

        public override int MySuggestions => 6;

        public override int MyWishlist => 6;

        public override int MyWatching => 6;
    }

    public class StandardRestrictions : Restrictions
    {
        public override int FavoriteProviders => 6;

        public override int MySuggestions => 12;

        public override int MyWishlist => 12;

        public override int MyWatching => 12;
    }

    public class PremiumRestrictions : Restrictions
    {
        public override int FavoriteProviders => 12;

        public override int MySuggestions => 24;

        public override int MyWishlist => 48;

        public override int MyWatching => 48;
    }
}