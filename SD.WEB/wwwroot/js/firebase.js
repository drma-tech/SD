"use strict";

import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
import {
    getAuth,
    onAuthStateChanged,
    setPersistence,
    browserLocalPersistence,
    GoogleAuthProvider,
    OAuthProvider,
    TwitterAuthProvider,
    signInWithPopup,
    signInWithRedirect,
} from "https://www.gstatic.com/firebasejs/12.6.0/firebase-auth.js";
import {
    getMessaging,
    onMessage,
    getToken,
} from "https://www.gstatic.com/firebasejs/12.6.0/firebase-messaging.js";

import { isBot, isLocalhost, isPrintScreen, firebaseConfig } from "./main.js";
import { storage, notification, interop } from "./utils.js";

//Xiaomi: The international model should work. The Chinese model perhaps not (and is likely to stop working completely in the near future).
//Huawei: It no longer offers GMS (Google Mobile Services) because it was blocked by Google. Implement: Huawei Push Kit
const nativePlatforms = new Set(["ios", "play", "xiaomi"]);
const platform = storage.getLocalStorage("platform");

if (!isBot && !isPrintScreen) {
    const app = initializeApp(firebaseConfig);
    const auth = getAuth(app);
    await setPersistence(auth, browserLocalPersistence);

    window.auth = auth;
    let refreshTokenInterval = null;

    onAuthStateChanged(auth, async (user) => {
        let token = user ? await user.getIdToken() : null;

        await interop.invokeDotNetWhenReady("SD.WEB", "AuthChanged", token);

        let objUser = authentication.getUser();

        if (objUser) {
            // services

            try {
                window.Userback.identify(objUser.userId, {
                    name: objUser.name,
                    email: objUser.email,
                });
            } catch {
                //ignores
            }

            // refresh token

            if (!refreshTokenInterval) {
                refreshTokenInterval = setInterval(
                    async () => {
                        const user = window.auth.currentUser;
                        if (user) {
                            const refreshedToken = await user.getIdToken(true);
                            await interop.invokeDotNetWhenReady("SD.WEB", "AuthChanged", refreshedToken);
                        } else {
                            await interop.invokeDotNetWhenReady("SD.WEB", "AuthChanged", null);
                            refreshTokenInterval = null;
                        }
                    },
                    30 * 60 * 1000
                );
            }
        } else {
            refreshTokenInterval = null;
        }
    });

    // Initialize messaging only for non-native platforms
    if (!nativePlatforms.has(platform)) {
        const messaging = getMessaging(app);
        window.messaging = messaging;

        onMessage(messaging, (payload) => {
            if (Notification.permission === "granted") {
                const { title, body } = payload.data || {};
                new Notification(title, { body });
            }
        });
    }
}

export const authentication = {
    async signIn(providerName) {
        let usePopup = false;
        if (isLocalhost) usePopup = true;
        if (platform === "ios") usePopup = true;

        const providerMap = {
            google: new GoogleAuthProvider(),
            apple: new OAuthProvider("apple.com"),
            microsoft: new OAuthProvider("microsoft.com"),
            yahoo: new OAuthProvider("yahoo.com"),
            x: new TwitterAuthProvider(),
        };

        const provider = providerMap[providerName];
        if (!provider) {
            throw new Error(`Unsupported provider: ${providerName}`);
        }

        async function doSignIn() {
            if (usePopup) {
                return signInWithPopup(window.auth, provider);
            } else {
                return signInWithRedirect(window.auth, provider);
            }
        }

        try {
            return await doSignIn();
        } catch (error) {
            const code = error.code || "";

            if (code === "auth/network-request-failed") {
                notification.sendLog(
                    "Network error detected. Retrying sign in..."
                );
                notification.showError(
                    "Network error detected. Retrying sign in..."
                );

                await new Promise((r) => setTimeout(r, 1000));

                try {
                    return await doSignIn();
                } catch (retryError) {
                    notification.sendLog(retryError);
                    throw new Error(retryError.message);
                }
            } else if (code === "auth/popup-closed-by-user") {
                notification.showError(
                    "Sign-in popup was closed before completion."
                );
            } else if (code === "auth/popup-blocked") {
                notification.showError(
                    "Your browser blocked the sign in popup."
                );
            } else if (code === "auth/cancelled-popup-request") {
                notification.showError(
                    "Another sign in request is already open."
                );
            } else {
                notification.sendLog(error);
                throw new Error(error.message);
            }
        }
    },
    async signOut() {
        try {
            await window.auth.signOut();
        } catch (error) {
            notification.sendLog(error);
            throw new Error(error.message);
        }
    },
    getUser() {
        try {
            const user = window.auth.currentUser;

            if (!user) return null;

            return {
                userId: user.uid,
                name: user.displayName || null,
                email: user.email || null,
            };
        } catch (error) {
            notification.sendLog(error);
            notification.showError(error.message);
            return null;
        }
    },
};

export const messaging = {
    async requestMessagingPermission() {
        if (nativePlatforms.has(platform)) {
            console.log("Using native push, no web permission needed.");
            return;
        }

        const permission = await Notification.requestPermission();

        if (permission !== "granted") {
            console.warn("Notifications not allowed.");
            return;
        }

        const token = await getToken(window.messaging, {
            vapidKey: firebaseConfig.messagingKey,
        });

        if (token) {
            storage.setLocalStorage("MessagingToken", token);
        } else {
            notification.showError("Failed to register messaging token.");
            return;
        }

        await interop.invokeDotNetWhenReady("SD.WEB", "SubscribeToTopics", {
            token,
            platform,
        });
    },
};