const ua = navigator.userAgent;

window.browser = window?.bowser?.getParser
    ? window.bowser.getParser(ua)
    : null;

export const isBot = /google|baidu|bingbot|duckduckbot|teoma|slurp|yandex|toutiao|bytespider|applebot/i.test(ua);

function testBrowserVersion(rules, ignore = false, fallback = false) {
    if (ignore) return false;

    if (!window.browser) return fallback;

    return window.browser.satisfies(rules);
}

//browser versions not compatible with SIMD
export const hideBlazorIndex = testBrowserVersion(
    {
        chrome: "<91",
        edge: "<91",
        firefox: "<89",
        safari: "<16.4",
        opera: "<77",
    },
    /Mediapartners-Google/i.test(ua),
    false // uncertain environment → allow
);

//probably a bot, so doesnt support sw
export const disableServiceWorker = testBrowserVersion(
    {
        chrome: "<134",
    },
    false,
    true // uncertain environment → disable
);

export const isLocalhost = location.host.includes("localhost");
export const isDev = location.hostname.includes("dev.");
export const isWebview = /webtonative/i.test(ua);
export const isPrintScreen = location.href.includes("printscreen");

export const servicesConfig = {
    AnalyticsCode: "G-4PREF5QX1F",
    ClarityKey: "r2iwqdpwtv",
    UserBackToken: "A-A2J4M5NKCbDp1QyQe7ogemmmq",
    SentryDsn: "https://94ae67eb3fb0bc82327607ddd9d6aebb@o4510938040041472.ingest.us.sentry.io/4510938043711488",
};

export const supabaseConfig = {
    projectUrl: "https://mlsztbywzbbqqbwgplky.supabase.co",
    supabaseKey: "sb_publishable_kwSh9KlLSaccPHPd7ZsqGw_VGpAs73w",
};

export const baseApiUrl = isLocalhost ? "http://localhost:7071" : "";

// Disable robots for dev environment
if (isDev) {
    const meta = document.createElement("meta");
    meta.name = "robots";
    meta.content = "noindex, nofollow";
    document.head.appendChild(meta);
}
