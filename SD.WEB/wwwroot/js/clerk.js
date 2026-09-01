"use strict";

import { storage, interop } from "./utils.js";

async function getClerkLocalization(language) {
    switch (language) {
        case "pt":
            return (await import("https://cdn.jsdelivr.net/npm/@clerk/localizations/pt-BR/+esm")).ptBR;

        case "es":
            return (await import("https://cdn.jsdelivr.net/npm/@clerk/localizations/es-ES/+esm")).esES;

        case "fr":
            return (await import("https://cdn.jsdelivr.net/npm/@clerk/localizations/fr-FR/+esm")).frFR;

        case "de":
            return (await import("https://cdn.jsdelivr.net/npm/@clerk/localizations/de-DE/+esm")).deDE;

        case "it":
            return (await import("https://cdn.jsdelivr.net/npm/@clerk/localizations/it-IT/+esm")).itIT;

        default:
            return undefined;
    }
}

let authReadyResolve;
const authReadyPromise = new Promise((resolve) => {
    authReadyResolve = resolve;
});

async function ensureAuthReady() {
    await authReadyPromise;

    if (!window.clerk) {
        throw new Error("Auth initialization failed");
    }

    return window.clerk;
}

let clerkLoadPromise;

function loadClerkScript() {
    if (clerkLoadPromise) {
        return clerkLoadPromise;
    }

    clerkLoadPromise = new Promise((resolve, reject) => {
        const isLocalhost = window.location.hostname === "localhost";
        const isDev = location.hostname.includes("develop");

        const publishableKey = isLocalhost || isDev ? window.appConfig.clerkConfig.devPk : window.appConfig.clerkConfig.prdPk;

        const script = document.createElement("script");

        script.crossOrigin = "anonymous";
        script.type = "text/javascript";
        script.src = "https://central-kiwi-3030.clerk.accounts.dev/npm/@clerk/clerk-js@6/dist/clerk.browser.js";

        script.dataset.clerkPublishableKey = publishableKey;

        script.onload = () => {
            if (!window.Clerk) {
                reject(new Error("ClerkJS loaded but window.Clerk is undefined."));
                return;
            }

            resolve(window.Clerk);
        };

        script.onerror = () => {
            reject(new Error("Failed to load ClerkJS."));
        };

        document.head.appendChild(script);
    });

    return clerkLoadPromise;
}

async function initAuth() {
    const appLanguage = storage.getLocalStorage("app-language");
    const browserLanguage = (navigator.language || "en").split("-")[0];
    const localization = await getClerkLocalization(appLanguage ?? browserLanguage);

    const Clerk = await loadClerkScript();

    await Clerk.load({
        ui: {
            ClerkUI: window.__internal_ClerkUICtor
        },
        localization,
    });

    window.clerk = Clerk;
    window.clerkUser = Clerk.user;

    setupAuthListener(Clerk);

    authReadyResolve(); // any call to ensureAuthReady will now proceed
}

if (!window.appConfig.isBot && !window.appConfig.isPrintScreen) {
    setTimeout(async () => {
        try {
            await initAuth();
        } catch (err) {
            try {
                Sentry.captureException(err);
            } catch {
                // ignore
            }
            authReadyResolve();
        }
    }, 0);
} else {
    authReadyResolve();
}

function setupAuthListener(clerk) {
    clerk.addListener(async ({ session, user }) => {
        const authProvider = storage.getLocalStorage("auth");
        if (authProvider !== "clerk") return;

        setTimeout(async () => {
            if (user && window.Userback?.identify) {
                try {
                    window.Userback.identify(user.id, {
                        name: user.fullName,
                        email: user.primaryEmailAddress?.emailAddress,
                    });
                } catch {
                    //ignores
                }
            }
        }, 5000);

        const token = session ? await session.getToken() : null;

        await interop.invokeDotNetWhenReady("SD.WEB", "ClerkAuthChanged", token);
    });
}

export const authentication = {
    async signIn() {
        try {
            storage.setLocalStorage("auth", "clerk");
            const clerk = await ensureAuthReady();
            clerk.openSignIn({ withSignUp: true });
        } catch (error) {
            Sentry.captureException(error);
            throw error.message;
        }
    },
    async signOut() {
        try {
            const clerk = await ensureAuthReady();
            await clerk.signOut();
        } catch (error) {
            Sentry.captureException(error);
            throw error.message;
        }
    },
    async accountPopup() {
        try {
            const div = document.getElementById('user-profile');

            const clerk = await ensureAuthReady();
            clerk.openUserProfile(div);
        } catch (error) {
            Sentry.captureException(error);
            throw error.message;
        }
    }
};