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

let serviceRoleKey = null;

function initAuth() {
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

function setupAuthListener(supabase) {
    let _lastSupabaseToken = null;

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

        if (token === _lastSupabaseToken) {
            return;
        }

        _lastSupabaseToken = token;

        await interop.invokeDotNetWhenReady("SD.WEB", "SupabaseAuthChanged", token);
    });
}

export const authentication = {
    async createUser(id, email, name) {
        const supabase = await ensureAuthReady();

        const { data, error } = await supabase.auth.admin.createUser({
            id: id,
            email: email,
            //password: password,
            email_confirm: true,
            user_metadata: {
                name: name,
            },
        });

        if (error) {
            throw new Error(error.message);
        } else {
            return data.user.id;
        }
    },
    async signIn(providerName) {
        try {
            const redirectTo = window.location.origin;

            const baseOptions = {
                redirectTo,
                scopes: "openid email",
            };

            const providerOverrides = {
                google: {
                    scopes: "openid email profile",
                    queryParams: {
                        access_type: "offline",
                        prompt: "consent",
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
            Sentry.captureException(error);
            throw new Error(error.message);
        }
    },
    async sendEmail(email) {
        const supabase = await ensureAuthReady();
        const { error } = await supabase.auth.signInWithOtp({
            email: email,
        });

        if (error) {
            Sentry.captureException(error);
            throw new Error(error.message);
        }
    },
    async confirmCode(email, code) {
        const supabase = await ensureAuthReady();
        const { error } = await supabase.auth.verifyOtp({
            email: email,
            token: code,
            type: "email",
        });

        if (error) {
            Sentry.captureException(error);
            throw new Error(error.message);
        }
    },
    async signOut() {
        try {
            const supabase = await ensureAuthReady();
            await supabase.auth.signOut();
        } catch (error) {
            Sentry.captureException(error);
            throw new Error(error.message);
        }
    },
    async getUser() {
        try {
            const supabase = await ensureAuthReady();
            const { data, error } = await supabase.auth.getSession();
            let user = data?.session?.user;

            if (!user) return null;

            if (error) {
                Sentry.captureException(error);
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
            Sentry.captureException(error);
            notification.showError(error.message);
            return null;
        }
    },
};