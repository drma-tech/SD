"use strict";

import { storage, notification } from "./utils.js";

export const services = {
    initGoogleAnalytics(version) {
        if (window.appConfig?.isLocalhost === true) return;
        if (window.appConfig?.isDev === true) return;

        const PLATFORM = storage.getLocalStorage("platform");

        window.dataLayer = window.dataLayer || [];
        function gtag() {
            window.dataLayer.push(arguments);
        }
        gtag("js", new Date());

        const config = {
            app_version: version,
            platform: PLATFORM,
        };

        gtag("config", window.appConfig?.servicesConfig?.AnalyticsCode, config);
    },
    initMicrosoftClarity(code) {
        if (window.appConfig?.isLocalhost === true) return;
        if (window.appConfig?.isDev === true) return;

        (function (c, l, a, r, i, t, y) {
            c[a] = c[a] || function () { (c[a].q = c[a].q || []).push(arguments) };
            t = l.createElement(r); t.async = 1; t.src = "https://www.clarity.ms/tag/" + i;
            y = l.getElementsByTagName(r)[0]; y.parentNode.insertBefore(t, y);
        })(window, document, "clarity", "script", code);

        // TODO: Determine whether Clarity's cookies, session replay, and collected fields require prior consent in each browser/PWA jurisdiction; apply the relevant consent or objection controls before collection. For native wrappers, assess actual personal-information collection against the applicable Huawei mainland-China and Xiaomi GetApps rules instead of blocking every SDK by default. ATT is only for Apple's cross-company tracking. Store labels are disclosures, not consent.
        // TODO: Do not set analytics/ad storage to granted before required browser consent. Where consent is not the required legal basis, use the applicable lawful basis and honor objection/opt-out rights. Do not reuse browser consent defaults as a universal native-app rule. For native wrappers, apply Huawei mainland-China and Xiaomi GetApps pre-collection requirements only to covered personal-information flows; ATT remains limited to Apple's cross-company tracking. Support withdrawal or objection as applicable.
        let attempts = 0;
        const maxAttempts = 12;
        const clarityCheckInterval = setInterval(function () {
            if (window.clarity) {
                clearInterval(clarityCheckInterval);
                window.clarity("consentv2", {
                    ad_Storage: "granted",
                    analytics_Storage: "granted"
                });
                return;
            }

            attempts++;
            if (attempts >= maxAttempts) clearInterval(clarityCheckInterval);
        }, 5000);
    },
    initUserJot() {
        if (window.appConfig?.isBot === true) return;
        if (window.appConfig?.blazorSupported !== true) return;

        const browserLang = navigator.language || navigator.userLanguage;

        window.uj = window.uj || new Proxy(
            {},
            {
                get: (_, property) =>
                    property == "then"
                        ? 0
                        : (...args) => {
                            (window.$ujq = window.$ujq || []).push([property, ...args]);
                        }
            }
        );
        window.uj.init(window.appConfig?.servicesConfig?.UserJotToken, {
            widget: {
                position: 'left', // 'left' or 'right'
                theme: 'auto', // 'auto', 'light', or 'dark'
            },
            locale: storage.getLocalStorage("app-language") ?? browserLang.slice(0, 2),
        });

        const script = document.createElement("script");
        script.type = "module";
        script.async = true;
        script.src = "https://cdn.userjot.com/sdk/v3/uj.js";
        (document.head || document.body).appendChild(script);
    },
    initAdSense(adClient, adSlot, containerId) {
        if (window.appConfig?.isBot === true) return;
        if (window.appConfig?.isLocalhost === true) return;
        if (window.appConfig?.isDev === true) return;
        if (window.appConfig?.blazorSupported !== true) return;

        try {
            const container = document.getElementById(containerId);
            if (!container) return;

            container.innerHTML = ""; // remove old ad

            const ins = document.createElement("ins");
            ins.className = "adsbygoogle custom-ad";
            ins.setAttribute("data-ad-client", adClient);
            ins.setAttribute("data-ad-slot", adSlot);
            ins.setAttribute("data-ad-format", "auto");
            ins.setAttribute('data-full-width-responsive', false);
            container.appendChild(ins);

            (window.adsbygoogle = window.adsbygoogle || []).push({});
        } catch (error) {
            Sentry?.captureException(error);
            notification.showError(error.message);
        }
    },
    initYandex(id) {
        if (window.appConfig?.isBot === true) return;
        if (window.appConfig?.isLocalhost === true) return;
        if (window.appConfig?.isDev === true) return;
        if (window.appConfig?.blazorSupported !== true) return;

        window.yaContextCb = window.yaContextCb || [];
        window.yaContextCb.push(() => {
            Ya.Context.AdvManager.render({
                "blockId": id,
                "renderTo": "yandex_rtb_" + id
            })
        });
    }
};

services.initMicrosoftClarity(window.appConfig?.servicesConfig?.ClarityKey);
