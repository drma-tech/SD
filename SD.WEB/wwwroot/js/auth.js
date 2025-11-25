"use strict";

// =========================
//  FIREBASE INIT
// =========================

const firebaseConfig = {
    apiKey: "AIzaSyDj5LpsT7-bra4hvuvb5E_BPSlD7Wr29nQ",
    authDomain: "auth.streamingdiscovery.com",
    projectId: "streaming-discovery-4c483",
    storageBucket: "streaming-discovery-4c483.firebasestorage.app",
    messagingSenderId: "394152837411",
    appId: "1:394152837411:web:06b7216d2859bdc1224c96",
    measurementId: "G-JKXTVXM0EX"
};

window.initFirebase = () => {
    firebase.initializeApp(firebaseConfig);

    const auth = firebase.auth();
    const messaging = firebase.messaging();

    auth.setPersistence(firebase.auth.Auth.Persistence.LOCAL);

    // =========================
    //  AUTH HANDLERS
    // =========================

    auth.onAuthStateChanged(async (user) => {
        await AuthStateChanged(user);
    });

    window.firebaseAuth = {
        signIn: async (providerName, email) => {
            try {
                const providerMap = {
                    google: new firebase.auth.GoogleAuthProvider(),
                    apple: new firebase.auth.OAuthProvider("apple.com"),
                    facebook: new firebase.auth.FacebookAuthProvider(),
                    microsoft: new firebase.auth.OAuthProvider("microsoft.com"),
                    yahoo: new firebase.auth.OAuthProvider("yahoo.com"),
                    x: new firebase.auth.TwitterAuthProvider()
                };

                const provider = providerMap[providerName];
                if (!provider) throw new Error(`Unsupported provider: ${providerName}`);
                const platform = GetLocalStorage("platform");

                if (isLocalhost || platform == "ios") {
                    await auth.signInWithPopup(provider);
                }
                else {
                    await auth.signInWithRedirect(provider);
                }
            } catch (error) {
                sendLog(error);
                throw new Error(error.message);
            }
        },

        signOut: async () => {
            try {
                await auth.signOut();
            } catch (error) {
                sendLog(error);
                throw new Error(error.message);
            }
        },

        getUser: () => {
            return auth.currentUser;
        }
    };

    // =========================
    //  MESSAGING HANDLERS
    // =========================

    async function requestMessagingPermission() {
        const platform = GetLocalStorage("platform");

        //Xiaomi: The international model should work. The Chinese model perhaps not (and is likely to stop working completely in the near future).
        //Huawei: It no longer offers GMS (Google Mobile Services) because it was blocked by Google. Implement: Huawei Push Kit
        const nativePlatforms = ["ios", "play", "xiaomi"];
        if (nativePlatforms.includes(platform)) {
            console.log("Using native push, no web permission needed.");
            return;
        }

        const permission = await Notification.requestPermission();

        if (permission !== "granted") {
            console.warn("Notifications not allowed.");
            return;
        }

        const token = await messaging.getToken({
            vapidKey: "BJ31lWbRBbX3ZyyUHG_pQB7ZmjFtNeFjhbhuyMwUvotpXsTej5iloeSA7GdCbC7HUo314KtgMxIvXiwygAG8NhQ"
        });

        if (token) {
            SetLocalStorage("MessagingToken", token);
        }
        else {
            showError("Failed to register messaging token.");
            return;
        }

        await invokeDotNetWhenReady("SD.WEB", "SubscribeToTopics", { token, platform });
    }

    messaging.onMessage((payload) => {
        if (Notification.permission === 'granted') {
            const { title, body } = payload.data || {};
            new Notification(title, { body });
        }
    });
}

if (!isBot) {
    window.addEventListener('load', () => {
        setTimeout(initFirebase, 300);
    });
}

async function FirebaseSignIn(provider) {
    if (typeof firebaseAuth === "undefined" || !firebaseAuth) {
        showError("Login is temporarily unavailable. Please make sure you have a stable connection or try again later.");

        if (typeof firebase === "undefined") {
            showError("firebase is undefined in initFirebase");
            sendLog("firebase is undefined in initFirebase");
        }
        else if (!firebase) {
            showError("firebase is null in initFirebase");
            sendLog("firebase is null in initFirebase");
        }
        else if (typeof firebaseAuth === "undefined") {
            showError("firebaseAuth is undefined in FirebaseSignIn");
            sendLog("firebaseAuth is undefined in FirebaseSignIn");
        }
        else if (!firebaseAuth) {
            showError("firebaseAuth is null in FirebaseSignIn");
            sendLog("firebaseAuth is null in FirebaseSignIn");
        }

        return;
    }

    await firebaseAuth.signIn(provider);
}