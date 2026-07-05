"use strict";

const ua = navigator.userAgent;
window.browser = window.bowser?.getParser ? window.bowser.getParser(ua) : null;
const botUAs = ["google", "baidu", "bingbot", "duckduckbot", "teoma", "slurp", "yandex", "toutiao", "bytespider", "applebot", "crawler"];
const isBot = botUAs.some(bot => ua.toLowerCase().includes(bot)) || navigator.webdriver;

function testBrowserVersion(rules, ignore = false, fallback = false) {
    if (ignore) return false;

    if (!window.browser) return fallback;

    try {
        return window.browser.satisfies(rules);
    } catch {
        return fallback;
    }
}

const wasmSupported = typeof WebAssembly === "object";
const isLocalhost = window.location.hostname === "localhost";
const isPrerendering = window.location.hostname === "127.0.0.1"
const isDev = location.hostname.includes("develop");
const isWebview = /webtonative/i.test(ua);
const isPrintScreen = location.href.includes("printscreen");

//browser versions not compatible with SIMD
const simdNotSupported = testBrowserVersion(
    {
        chrome: "<91", //may 21
        edge: "<91", //may 21
        firefox: "<89", //may 21
        safari: "<16.4", //mar 23
        opera: "<77", //jun 21
    },
    /Mediapartners-Google/i.test(ua),
    false // uncertain environment → allow
);

//The browser does not support WASM or SIMD.
const blazorSupported = wasmSupported && !simdNotSupported;

//probably a bot, so doesnt support sw
const disableServiceWorker = testBrowserVersion(
    {
        chrome: "<134", //special case (usually bots)
        edge: "<91", //may 21
        firefox: "<89", //may 21
        safari: "<16.4", //mar 23
        opera: "<77", //jun 21
    },
    isWebview,
    true // uncertain environment → disable
);

const servicesConfig = {
    AnalyticsCode: "G-4PREF5QX1F",
    ClarityKey: "r2iwqdpwtv",
    UserBackToken: "A-A2J4M5NKCbDp1QyQe7ogemmmq",
    SentryDsn: "https://94ae67eb3fb0bc82327607ddd9d6aebb@o4510938040041472.ingest.us.sentry.io/4510938043711488",
};

const supabaseConfig = {
    projectUrl: "https://mlsztbywzbbqqbwgplky.supabase.co",
    supabaseKey: "sb_publishable_kwSh9KlLSaccPHPd7ZsqGw_VGpAs73w",
};

const baseApiUrl = isLocalhost ? "http://localhost:7071" : "";

window.appConfig = {
    isBot,
    blazorSupported,
    disableServiceWorker,
    isLocalhost,
    isPrerendering,
    isDev,
    isWebview,
    isPrintScreen,
    servicesConfig,
    supabaseConfig,
    baseApiUrl
};
