"use strict";

import { initializeApp } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-app.js";
import { getAuth, onAuthStateChanged, setPersistence, browserLocalPersistence, GoogleAuthProvider, OAuthProvider, FacebookAuthProvider, TwitterAuthProvider, signInWithPopup, signInWithRedirect } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-auth.js";
import { getMessaging, onMessage, getToken } from "https://www.gstatic.com/firebasejs/12.6.0/firebase-messaging.js";

const firebaseConfig = {
    apiKey: "AIzaSyDj5LpsT7-bra4hvuvb5E_BPSlD7Wr29nQ",
    authDomain: "auth.streamingdiscovery.com",
    projectId: "streaming-discovery-4c483",
    storageBucket: "streaming-discovery-4c483.firebasestorage.app",
    messagingSenderId: "394152837411",
    appId: "1:394152837411:web:06b7216d2859bdc1224c96",
    measurementId: "G-JKXTVXM0EX",
};

//Xiaomi: The international model should work. The Chinese model perhaps not (and is likely to stop working completely in the near future).
//Huawei: It no longer offers GMS (Google Mobile Services) because it was blocked by Google. Implement: Huawei Push Kit
const nativePlatforms = ["ios", "play", "xiaomi"];
const platform = GetLocalStorage("platform");

if (!isBot) {
    const app = initializeApp(firebaseConfig);
    const auth = getAuth(app);
    await setPersistence(auth, browserLocalPersistence);

    window.auth = auth;

    onAuthStateChanged(auth, async (user) => {
        if (isBot) return;

        let token = user ? await user.getIdToken() : null;

        await invokeDotNetWhenReady("SD.WEB", "AuthChanged", token);

        let objUser = getUser();

        if (objUser) {
            if (typeof Userback !== "undefined" && Userback) {
                Userback.identify(objUser.userId, {
                    name: objUser.name,
                    email: objUser.email,
                });
            }
        }
    });

    // Initialize messaging only for non-native platforms
    if (!nativePlatforms.includes(platform)) {
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

window.firebaseAuth = {
    signIn: async (providerName, email) => {
        let usePopup = false;
        if (isLocalhost) usePopup = true;
        if (platform === "ios") usePopup = true;

        const providerMap = {
            google: new GoogleAuthProvider(),
            apple: new OAuthProvider("apple.com"),
            facebook: new FacebookAuthProvider(),
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
                return signInWithPopup(window.auth, provider)
            } else {
                return signInWithRedirect(window.auth, provider);
            }
        }

        try {
            return await doSignIn();
        } catch (error) {
            const isNetworkError = error.message.includes("auth/network-request-failed");

            if (isNetworkError) {
                sendLog("Network error detected. Retrying sign in...");
                showError("Network error detected. Retrying sign in...");

                await new Promise(r => setTimeout(r, 1000));

                try {
                    return await doSignIn();
                } catch (retryError) {
                    sendLog(retryError);
                    throw new Error(retryError.message);
                }
            } else {
                sendLog(error);
                throw new Error(error.message);
            }
        }
    },

    signOut: async () => {
        try {
            await window.auth.signOut();
        } catch (error) {
            sendLog(error);
            throw new Error(error.message);
        }
    },

    getUser: () => {
        return window.auth.currentUser;
    },
};

window.requestMessagingPermission = async function () {
    if (nativePlatforms.includes(platform)) {
        console.log("Using native push, no web permission needed.");
        return;
    }

    const permission = await Notification.requestPermission();

    if (permission !== "granted") {
        console.warn("Notifications not allowed.");
        return;
    }

    const token = await getToken(window.messaging, {
        vapidKey: "BJ31lWbRBbX3ZyyUHG_pQB7ZmjFtNeFjhbhuyMwUvotpXsTej5iloeSA7GdCbC7HUo314KtgMxIvXiwygAG8NhQ",
    });

    if (token) {
        SetLocalStorage("MessagingToken", token);
    } else {
        showError("Failed to register messaging token.");
        return;
    }

    await invokeDotNetWhenReady("SD.WEB", "SubscribeToTopics", { token, platform, });
};