window.browser = window.bowser.getParser(window.navigator.userAgent);

export const isBot =
    /google|baidu|bingbot|duckduckbot|teoma|slurp|yandex|toutiao|bytespider|applebot/i.test(
        navigator.userAgent
    );

// supports WebAssembly SIMD
export const isOldBrowser = window.browser.satisfies({
    chrome: "<91",
    edge: "<91",
    safari: "<16.4",
});
export const isLocalhost = location.host.includes("localhost");
export const isDev = location.hostname.includes("dev.");
export const isWebview = /webtonative/i.test(navigator.userAgent);
export const isPrintScreen = location.href.includes("printscreen");
export const appVersion = "version-error";

export const servicesConfig = {
    AnalyticsCode: "G-4PREF5QX1F",
    ClarityKey: "r2iwqdpwtv",
    UserBackToken: "A-A2J4M5NKCbDp1QyQe7ogemmmq",
    UserBackSurveyKey: "mjj9Ta",
};

export const firebaseConfig = {
    apiKey: "AIzaSyDj5LpsT7-bra4hvuvb5E_BPSlD7Wr29nQ",
    authDomain: "auth.streamingdiscovery.com",
    projectId: "streaming-discovery-4c483",
    storageBucket: "streaming-discovery-4c483.firebasestorage.app",
    messagingSenderId: "394152837411",
    messagingKey: "BJ31lWbRBbX3ZyyUHG_pQB7ZmjFtNeFjhbhuyMwUvotpXsTej5iloeSA7GdCbC7HUo314KtgMxIvXiwygAG8NhQ",
    appId: "1:394152837411:web:06b7216d2859bdc1224c96",
    measurementId: "G-JKXTVXM0EX",
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