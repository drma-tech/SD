namespace SD.Shared.Core.Helper;

public static class SubscriptionHelper
{
    public static Restrictions GetRestrictions(this AccountProduct product)
    {
        return product switch
        {
            AccountProduct.Basic => new BasicRestrictions(),
            AccountProduct.Premium => new PremiumRestrictions(),
            _ => new BasicRestrictions()
        };
    }

    public static void ValidateFavoriteProviders(AccountProduct? product, int qtd)
    {
        product ??= AccountProduct.Basic;
        var restriction = product.Value.GetRestrictions();

        if (qtd > restriction.FavoriteProviders)
            throw new NotificationException("Your current plan does not support this operation. Consider upgrading to premium for more benefits.");
    }

    public static void ValidateWatched(AccountProduct? product, int qtd)
    {
        product ??= AccountProduct.Basic;
        var restriction = product.Value.GetRestrictions();

        if (qtd > restriction.Watched)
            throw new NotificationException("Your current plan does not support this operation. Consider upgrading to premium for more benefits.");
    }

    public static void ValidateWatching(AccountProduct? product, int qtd)
    {
        product ??= AccountProduct.Basic;
        var restriction = product.Value.GetRestrictions();

        if (qtd > restriction.Watching)
            throw new NotificationException("Your current plan does not support this operation. Consider upgrading to premium for more benefits.");
    }

    public static void ValidateWishList(AccountProduct? product, int qtd)
    {
        product ??= AccountProduct.Basic;
        var restriction = product.Value.GetRestrictions();

        if (qtd > restriction.Wishlist)
            throw new NotificationException("Your current plan does not support this operation. Consider upgrading to premium for more benefits.");
    }
}

public abstract class Restrictions
{
    public abstract int Energy { get; }
    public abstract int FavoriteProviders { get; }
    public abstract int MySuggestions { get; }
    public abstract int Watched { get; } //30x
    public abstract int Watching { get; } //4x
    public abstract int Wishlist { get; } //4x
}

public class BasicRestrictions : Restrictions
{
    public override int Energy => 10;
    public override int FavoriteProviders => 2;
    public override int MySuggestions => 0;
    public override int Watched => 14;
    public override int Watching => 7;
    public override int Wishlist => 7;
}

public class PremiumRestrictions : Restrictions
{
    public override int Energy => 100;
    public override int FavoriteProviders => 20;
    public override int MySuggestions => 20;
    public override int Watched => 600;
    public override int Watching => 80;
    public override int Wishlist => 80;
}