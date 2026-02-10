"use strict";

import { createClient } from "https://cdn.jsdelivr.net/npm/@supabase/supabase-js/+esm";

import { isBot, isPrintScreen, supabaseConfig } from "./main.js";
import { storage, notification, interop } from "./utils.js";

let authReadyResolve;
const authReadyPromise = new Promise((resolve) => {
    authReadyResolve = resolve;
});

async function ensureAuthReady() {
    await authReadyPromise;

    if (!window.supabase) {
        throw new Error("Auth initialization failed");
    }

    return window.supabase;
}

function initAuth() {
    let serviceRoleKey = null;

    const supabase = createClient(
        supabaseConfig.projectUrl,
        serviceRoleKey ?? supabaseConfig.supabaseKey,
        {
            auth: {
                persistSession: true,
                autoRefreshToken: true,
                detectSessionInUrl: true,
            },
        }
    );

    window.supabase = supabase;
    setupAuthListener(supabase);

    authReadyResolve(); // any call to ensureAuthReady will now proceed
}

if (!isBot && !isPrintScreen) {
    setTimeout(() => {
        try {
            initAuth();
        } catch (err) {
            try {
                notification.sendLog(err);
            } catch {
                // ignore
            }
            authReadyResolve();
        }
    }, 0);
} else {
    authReadyResolve();
}

function setupAuthListener(supabase) {
    supabase.auth.onAuthStateChange(async (event, session) => {
        const authProvider = storage.getLocalStorage("auth");
        if (authProvider !== "supabase") return;

        let user = session?.user;

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

        const token = session?.access_token ?? null;
        await interop.invokeDotNetWhenReady("SD.WEB", "SupabaseAuthChanged", token);
    });
}

export const authentication = {
    async createUser(id, email, name) {
        const supabase = await ensureAuthReady();

        const { error } = await supabase.auth.admin.createUser({
            id: id,
            email: email,
            //password: password,
            email_confirm: true,
            user_metadata: {
                name: name,
            },
        });

        if (error) {
            notification.sendLog(error);
            notification.showError(error.message);
        }
    },
    async signIn(providerName) {
        try {
            const redirectTo = window.location.origin;
            const platform = storage.getLocalStorage("platform");

            const baseOptions = {
                redirectTo,
                scopes: "openid email",
            };

            const providerOverrides = {
                google: {
                    scopes: "openid email profile",
                    queryParams: {
                        prompt: "consent",
                        ...(platform === "ios"
                            ? {}
                            : { access_type: "offline" }),
                    },
                },
                apple: {
                    scopes: "email name",
                },
                azure: {
                    scopes: "openid email offline_access",
                },
            };

            const providerOptions = {
                ...baseOptions,
                ...providerOverrides[providerName],
            };

            const supabase = await ensureAuthReady();

            supabase.auth.signInWithOAuth({
                provider: providerName,
                options: providerOptions,
            });
        } catch (error) {
            notification.sendLog(error);
            throw new Error(error.message);
        }
    },
    async sendEmail(email) {
        const { error } = await window.supabase.auth.signInWithOtp({
            email: email,
        });

        if (error) {
            notification.sendLog(error);
            notification.showError(error.message);
        }
    },
    async confirmCode(email, code) {
        const { error } = await window.supabase.auth.verifyOtp({
            email: email,
            token: code,
            type: "email",
        });

        if (error) {
            notification.sendLog(error);
            notification.showError(error.message);
        }
    },
    async signOut() {
        try {
            await window.supabase.auth.signOut();
        } catch (error) {
            notification.sendLog(error);
            throw new Error(error.message);
        }
    },
    async getUser() {
        try {
            const { data, error } = await window.supabase.auth.getSession();
            let user = data?.session?.user;

            if (!user) return null;

            if (error) {
                notification.sendLog(error);
                notification.showError(error.message);
                return null;
            } else {
                return {
                    userId: user.id,
                    name: user.user_metadata.full_name || null,
                    email: user.email || null,
                };
            }
        } catch (error) {
            notification.sendLog(error);
            notification.showError(error.message);
            return null;
        }
    },
};