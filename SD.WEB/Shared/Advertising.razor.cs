using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace SD.WEB.Shared
{
    public enum AdNetwork
    {
        None = 0,
        Adsense = 1,
        Yandex = 2,
    }

    public enum AdSlot
    {
        Header,
        Middle,
        Footer,
    }

    public partial class Advertising
    {
        [Parameter][SupplyParameterFromQuery(Name = "printscreen")] public bool Printscreen { get; set; } = false;
        [Parameter][SupplyParameterFromQuery(Name = "country")] public string? Country { get; set; }
        [Parameter][EditorRequired] public AdSlot Slot { get; set; }
        [Parameter] public string[] IgnoreRoutes { get; set; } = ["auth", "legal"];

        private static bool ActiveSubscription => AppStateStatic.Principal?.GetActiveSubscription() != null;

        private string? country { get; set; }
        private AdNetwork Network { get; set; } = AdNetwork.None;
        private const string AdClientId = "5145928155833172";

        private bool _adInitialized;
        private string? _lastUri;
        private string _containerId = $"ad-container-{Guid.NewGuid()}";

        private readonly HashSet<string> _AdSenseBlockedCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            "CU", // Cuba
            "IR", // Iran
            "KP", // North Korea
        };

        //disabled for now (enable if users increase)
        private readonly HashSet<string> _YandexCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            // "RU", // Russia
            // "BY", // Belarus
            // "KZ", // Kazakhstan
            // "AM", // Armenia
            // "AZ", // Azerbaijan
            // "KG", // Kyrgyzstan
            // "TJ", // Tajikistan
            // "UZ", // Uzbekistan
            // "MD", // Moldova
            // "TM"  // Turkmenistan
        };

        private static string GetAdSenseId(AdSlot slot) => _adsenseSlots[slot];

        private static readonly Dictionary<AdSlot, string> _adsenseSlots = new()
        {
            { AdSlot.Header, "7737592628" },
            { AdSlot.Middle, "6324586995" },
            { AdSlot.Footer, "4604311219" },
        };

        private static string GetYandexSlotId(AdSlot slot) => _yandexSlots[slot];

        private static readonly Dictionary<AdSlot, string> _yandexSlots = new()
        {
            { AdSlot.Header, "R-A-19337518-2" },
            { AdSlot.Middle, "R-A-19337518-1" },
            { AdSlot.Footer, "R-A-19337518-3" },
        };

        protected override void OnInitialized()
        {
            Navigation.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (string.Equals(_lastUri, e.Location, StringComparison.Ordinal))
                return;

            _lastUri = e.Location;

            _adInitialized = false;
            _containerId = $"ad-container-{Guid.NewGuid()}";

            _ = InvokeAsync(StateHasChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    //detect country
                    country = Country ?? await AppStateStatic.GetCountry(IpInfoApi, JsRuntime, Cts.Token);

                    //detect network
                    if (Navigation.IsLocalhost() || Printscreen || ActiveSubscription)
                    {
                        Network = AdNetwork.None;
                    }
                    else
                    {
                        if (country.NotEmpty() && _YandexCountries.Contains(country))
                        {
                            Network = AdNetwork.Yandex;
                        }
                        else if (country.NotEmpty() && _AdSenseBlockedCountries.Contains(country))
                        {
                            Network = AdNetwork.None;
                        }
                        else //adsense
                        {
                            Network = AdNetwork.Adsense;
                        }
                    }

                    StateHasChanged(); //re-render to create ad container
                }
                else if (Network != AdNetwork.None)
                {
                    if (_adInitialized)
                        return;

                    _adInitialized = true;

                    if (Network == AdNetwork.Adsense)
                    {
                        await JsRuntime.Services().InitAdSense($"ca-pub-{AdClientId}", GetAdSenseId(Slot), _containerId, Cts.Token);
                        _ = Task.Run(() => DetectAdBlockAsync(Cts.Token));
                    }
                    else if (Network == AdNetwork.Yandex)
                    {
                        await JsRuntime.Services().InitYandex(GetYandexSlotId(Slot), Cts.Token);
                    }
                }

                await base.OnAfterRenderAsync(firstRender);
            }
            catch (Exception ex)
            {
                await ProcessException(ex);
            }
        }

        private async Task DetectAdBlockAsync(CancellationToken cancellationToken)
        {
            var blocked = await JsRuntime.Utils().IsAdBlocked(cancellationToken);

            if (blocked)
            {
                await InvokeAsync(async () =>
                {
                    await ShowWarning(Translations.Notification.AdBlockerDetected);
                });
            }
        }

        private async Task OpenSubscription()
        {
            await DialogService.SubscriptionPopup();
        }
    }
}