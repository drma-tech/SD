"use strict";

import { storage, notification, interop } from "./utils.js";

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

        const publishableKey = isLocalhost || isDev ? "pk_test_Y2VudHJhbC1raXdpLTMwMzAuY2xlcmsuYWNjb3VudHMuZGV2JA" : "pk_live_Y2xlcmsuc3RyZWFtaW5nZGlzY292ZXJ5LmNvbSQ";

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
    let _lastToken = null;

    clerk.addListener(async ({ user }) => {
        const authProvider = storage.getLocalStorage("auth");
        if (authProvider !== "clerk") return;

        setTimeout(async () => {
            if (user && window.Userback?.identify) {
                try {
                    window.Userback.identify(user.id, {
                        name: user.user_metadata.full_name,
                        email: user.email,
                    });
                } catch {
                    //ignores
                }
            }
        }, 1000);

        const token = user ? await clerk.session?.getToken() : null;

        if (token === _lastToken) {
            return;
        }

        _lastToken = token;

        await interop.invokeDotNetWhenReady("SD.WEB", "ClerkAuthChanged", token);
    });
}

export const authentication = {
    // async createUser(id, email, name) {
    //     const supabase = await ensureAuthReady();

    //     const { data, error } = await supabase.auth.admin.createUser({
    //         id: id,
    //         email: email,
    //         //password: password,
    //         email_confirm: true,
    //         user_metadata: {
    //             name: name,
    //         },
    //     });

    //     if (error) {
    //         throw error.message;
    //     } else {
    //         return data.user.id;
    //     }
    // },
    async signIn() {
        try {
            storage.setLocalStorage("auth", "clerk");
            const clerk = await ensureAuthReady();
            clerk.openSignIn();
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
    // async getUser() {
    //     try {
    //         const supabase = await ensureAuthReady();
    //         const { data, error } = await supabase.auth.getSession();
    //         let user = data?.session?.user;

    //         if (!user) return null;

    //         if (error) {
    //             Sentry.captureException(error);
    //             notification.showError(error.message);
    //             return null;
    //         } else {
    //             return {
    //                 userId: user.id,
    //                 name: user.user_metadata.full_name || null,
    //                 email: user.email || null,
    //                 avatar: user.user_metadata.avatar_url
    //             };
    //         }
    //     } catch (error) {
    //         Sentry.captureException(error);
    //         notification.showError(error.message);
    //         return null;
    //     }
    // },
};