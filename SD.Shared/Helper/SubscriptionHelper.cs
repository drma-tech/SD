namespace SD.Shared.Helper
{
    public static class SubscriptionHelper
    {
        public static Restrictions GetRestrictions(this AccountProduct product)
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
            product ??= AccountProduct.Basic;
            var restriction = GetRestrictions(product.Value);

            if (qtd > restriction.FavoriteProviders)
            {
                throw new NotificationException("Your current plan does not support this operation");
            }
        }

        public static void ValidateWatched(AccountProduct? product, int qtd)
        {
            product ??= AccountProduct.Basic;
            var restriction = GetRestrictions(product.Value);

            if (qtd > restriction.Watched)
            {
                throw new NotificationException("Your current plan does not support this operation");
            }
        }

        public static void ValidateWatching(AccountProduct? product, int qtd)
        {
            product ??= AccountProduct.Basic;
            var restriction = GetRestrictions(product.Value);

            if (qtd > restriction.Watching)
            {
                throw new NotificationException("Your current plan does not support this operation");
            }
        }

        public static void ValidateWishList(AccountProduct? product, int qtd)
        {
            product ??= AccountProduct.Basic;
            var restriction = GetRestrictions(product.Value);

            if (qtd > restriction.Wishlist)
            {
                throw new NotificationException("Your current plan does not support this operation");
            }
        }
    }

    public abstract class Restrictions
    {
        public abstract int FavoriteProviders { get; }
        public abstract int MySuggestions { get; }
        public abstract int Watched { get; }
        public abstract int Watching { get; }
        public abstract int Wishlist { get; }
    }

    public class BasicRestrictions : Restrictions
    {
        public override int FavoriteProviders => 1;
        public override int MySuggestions => 0;
        public override int Watched => 10;
        public override int Watching => 6;
        public override int Wishlist => 6;
    }

    public class StandardRestrictions : Restrictions
    {
        public override int FavoriteProviders => 12;
        public override int MySuggestions => 12;
        public override int Watched => 200;
        public override int Watching => 50;
        public override int Wishlist => 50;
    }

    public class PremiumRestrictions : Restrictions
    {
        public override int FavoriteProviders => 24;
        public override int MySuggestions => 24;
        public override int Watched => 400;
        public override int Watching => 100;
        public override int Wishlist => 100;
    }
}