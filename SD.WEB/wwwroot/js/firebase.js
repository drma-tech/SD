"use strict";

//https://firebase.google.com/support/release-notes/js

const MAX_RETRIES = 2;
const RETRY_TIMEOUT = 1000;

import { initializeApp } from "https://www.gstatic.com/firebasejs/12.8.0/firebase-app.js";
import {
    getAuth,
    onAuthStateChanged,
    onIdTokenChanged,
    setPersistence,
    browserLocalPersistence,
    GoogleAuthProvider,
    OAuthProvider,
    TwitterAuthProvider,
    signInWithPopup,
    signInWithRedirect,
} from "https://www.gstatic.com/firebasejs/12.8.0/firebase-auth.js";

import { isBot, isLocalhost, isPrintScreen, firebaseConfig } from "./main.js";
import { storage, notification, interop } from "./utils.js";

const platform = storage.getLocalStorage("platform");

let authReadyResolve;
const authReadyPromise = new Promise((resolve) => {
    authReadyResolve = resolve;
});

async function ensureAuthReady() {
    await authReadyPromise;

    if (!window.firebase) {
        throw new Error("Auth initialization failed");
    }

    return window.firebase;
}

async function initAuth() {
    const app = initializeApp(firebaseConfig);
    const auth = getAuth(app);
    await setPersistence(auth, browserLocalPersistence);
    window.firebase = auth;
    setupAuthListener(auth);
    authReadyResolve(); // any call to ensureAuthReady will now proceed
}

if (!isBot && !isPrintScreen) {
    initAuth();
} else {
    authReadyResolve();
}

function setupAuthListener(auth) {
    //sign-in or sign-out
    onAuthStateChanged(auth, async (user) => {
        const authProvider = storage.getLocalStorage("auth");
        if (authProvider !== "firebase") return;

        if (user && window.Userback?.identify) {
            try {
                window.Userback.identify(user.uid, {
                    name: user.displayName,
                    email: user.email,
                });
            } catch {
                //ignores
            }
        }
    });

    //sign-in, sign-out, and token refresh events
    onIdTokenChanged(auth, async (user) => {
        try {
            const authProvider = storage.getLocalStorage("auth");
            if (authProvider !== "firebase") return;

            const token = user ? await user.getIdToken() : null;
            await interop.invokeDotNetWhenReady("SD.WEB", "FirebaseAuthChanged", token);
        } catch (err) {
            notification.sendLog(err);
        }
    });
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
            const auth = await ensureAuthReady();

            if (usePopup) {
                return signInWithPopup(auth, provider);
            } else {
                await signInWithRedirect(auth, provider);
                return null;
            }
        }

        async function doSignInWithRetry(retryCount = 0) {
            try {
                return await doSignIn();
            } catch (error) {
                if (
                    error.code === "auth/network-request-failed" &&
                    retryCount < MAX_RETRIES
                ) {
                    notification.sendLog(
                        "Network error detected. Retrying sign in..."
                    );
                    notification.showError(
                        "Network error detected. Retrying sign in..."
                    );

                    await new Promise((r) =>
                        setTimeout(r, RETRY_TIMEOUT * Math.pow(2, retryCount))
                    );
                    return doSignInWithRetry(retryCount + 1);
                }
                throw error;
            }
        }

        try {
            return await doSignInWithRetry();
        } catch (error) {
            const code = error.code || "";

            if (code === "auth/popup-closed-by-user") {
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
            await window.firebase.signOut();
        } catch (error) {
            notification.sendLog(error);
            throw new Error(error.message);
        }
    },
    getUser() {
        try {
            const user = window.firebase.currentUser;

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