"use strict";

import { createClient } from "https://cdn.jsdelivr.net/npm/@supabase/supabase-js/+esm";

import { isBot, isPrintScreen, supabaseConfig } from "./main.js";
import { storage, notification, interop } from "./utils.js";

async function initAuth() {
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
    setupAuthListener();
}

if (!isBot && !isPrintScreen) {
    await initAuth();
}

function setupAuthListener() {
    window.supabase.auth.onAuthStateChange((event, session) => {
        const authProvider = storage.getLocalStorage("auth");
        if (authProvider !== "supabase") return;

        const token = session?.access_token ?? null;
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

        interop.invokeDotNetWhenReady("SD.WEB", "SupabaseAuthChanged", token);
    });
}

export const authentication = {
    async createUser(id, email, name) {
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
            notification.sendLog(error);
            notification.showError(error.message);
        }
    },
    async signIn(providerName) {
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

        window.supabase.auth.signInWithOAuth({
            provider: providerName,
            options: providerOptions,
        });
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