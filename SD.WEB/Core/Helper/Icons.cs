namespace SD.WEB.Core.Helper
{
    public enum IconAnimation
    {
        /// <summary>
        /// No animation.
        /// </summary>
        None,

        /// <summary>
        /// Draw attention to positive, important, or active states. Example: favorites, likes, health, success indicators.
        /// </summary>
        Beat,

        /// <summary>
        /// Strong attention-grabbing pulse effect. Example: notifications, alerts, promotions, urgent actions.
        /// </summary>
        BeatFade,

        /// <summary>
        /// Indicates excitement, celebration, or newly available content. Example: rewards, achievements, special events.
        /// </summary>
        Bounce,

        /// <summary>
        /// Subtle attention indicator. Example: unread notifications, status indicators, live updates.
        /// </summary>
        Fade,

        /// <summary>
        /// Represents switching, turning over, or changing state. Example: refresh actions, card flips, view toggles.
        /// </summary>
        Flip,

        /// <summary>
        /// Indicates an error, warning, or invalid action. Example: validation errors, access denied, incorrect input.
        /// </summary>
        Shake,

        /// <summary>
        /// Indicates ongoing work or processing. Example: loading, syncing, refreshing.
        /// </summary>
        Spin,

        /// <summary>
        /// Same as Spin, but rotating in the opposite direction. Useful when multiple spinners appear together.
        /// </summary>
        SpinReverse,

        /// <summary>
        /// Stepped spinner animation. Best suited for traditional loading indicators.
        /// </summary>
        SpinPulse
    }

    public static class IconsFA
    {
        public static class Solid
        {
            public static Icon Icon(string? name) => new("fa-solid", name ?? "");
        }
    }

    public class Icon(string category, string name, IconAnimation animation = IconAnimation.None)
    {
        public string Category { get; set; } = category;
        public string Name { get; set; } = name;

        public IconAnimation IconAnimation { get; set; } = animation;

        public Icon Animation(IconAnimation animation)
        {
            IconAnimation = animation;
            return this;
        }

        public string? Font => IconHelper.GetFont(this);
    }

    public static class IconHelper
    {
        public static string? GetFont(Icon icon)
        {
            if (icon.Name.Empty()) return null;

            var animationClass = icon.IconAnimation switch
            {
                IconAnimation.Beat => "fa-beat",
                IconAnimation.BeatFade => "fa-beat-fade",
                IconAnimation.Bounce => "fa-bounce",
                IconAnimation.Fade => "fa-fade",
                IconAnimation.Flip => "fa-flip",
                IconAnimation.Shake => "fa-shake",
                IconAnimation.Spin => "fa-spin",
                IconAnimation.SpinReverse => "fa-spin-reverse",
                IconAnimation.SpinPulse => "fa-spin-pulse",
                _ => null
            };

            return $"{icon.Category} fa-{icon.Name} {animationClass}".Trim();
        }
    }
}