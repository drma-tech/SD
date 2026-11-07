"use strict";

import { storage, environment } from "./utils.js";

const env = (() => {
    if (window.appConfig?.isLocalhost === true) return "development";
    if (window.appConfig?.isDev === true) return "staging";
    return "production";
})();

const ignoredErrors = [
    /failed to fetch/i,
    /failed to register/i,
    /failed to start/i,
    /token has expired/i,
];

window.sentryOnLoad = function () {
    Sentry.init({
        dsn: window.appConfig?.servicesConfig?.SentryDsn,
        release: `sd-js@${window.appVersion}`,
        environment: env,
        ignoreErrors: ignoredErrors,
        beforeSend(event) {
            event.tags = {
                ...event.tags,
                "custom.version": window.appVersion,
                "custom.platform": storage.getLocalStorage("platform") ?? "unknown",
                "custom.isAdBlocked": window.isAdBlocked ?? "unknown",
                "custom.isAuthenticated": window.isAuthenticated ?? false,
                "custom.isBot": window.appConfig?.isBot ?? "unknown",
                "custom.blazorSupported": window.appConfig?.blazorSupported ?? "unknown",
                "custom.disableServiceWorker": window.appConfig?.disableServiceWorker ?? "unknown",
            };
            event.extra = {
                ...event.extra,
                browser_name: environment.getBrowserName() ?? "unknown",
                browser_version: environment.getBrowserVersion() ?? "unknown",
                operation_system: environment.getOperatingSystem() ?? "unknown",
            };

            // TODO: Review Sentry's actual diagnostic payloads, personal data, URLs, query strings, identifiers, recipients, and retention. Minimize collected context and document the lawful basis and required notice/objection controls. Gate initialization before collection only where a specific legal or store rule applies to the actual diagnostic data; do not assume crash reporting universally requires a consent popup. Store privacy labels are disclosures, not consent.
            if (isOpaqueScriptError(event)) {
                event.tags["custom.opaqueScriptError"] = "true";

                event.extra = {
                    ...event.extra,
                    page_url: window.location.href,
                    recent_scripts: getRecentScripts(),
                };
            }

            return event;
        },
    });
}

function getRecentScripts(limit = 20) {
    return performance
        .getEntriesByType("resource")
        .filter(entry => entry.initiatorType === "script")
        .slice(-limit)
        .map(entry => {
            try {
                const url = new URL(entry.name);

                return {
                    url: `${url.origin}${url.pathname}`,
                    duration_ms: Math.round(entry.duration),
                };
            } catch {
                return {
                    url: "unknown",
                    duration_ms: Math.round(entry.duration),
                };
            }
        });
}

function isOpaqueScriptError(event) {
    const messages = [
        event.message,
        ...(event.exception?.values ?? []).map(exception => exception.value),
    ];

    return messages.some(message => message === "Script error.");
}