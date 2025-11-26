"use strict";

const firebaseConfig = {
    apiKey: "AIzaSyDj5LpsT7-bra4hvuvb5E_BPSlD7Wr29nQ",
    authDomain: "auth.streamingdiscovery.com",
    projectId: "streaming-discovery-4c483",
    storageBucket: "streaming-discovery-4c483.firebasestorage.app",
    messagingSenderId: "394152837411",
    appId: "1:394152837411:web:06b7216d2859bdc1224c96",
    measurementId: "G-JKXTVXM0EX"
};

//Xiaomi: The international model should work. The Chinese model perhaps not (and is likely to stop working completely in the near future).
//Huawei: It no longer offers GMS (Google Mobile Services) because it was blocked by Google. Implement: Huawei Push Kit
const nativePlatforms = ["ios", "play", "xiaomi"];
const platform = GetLocalStorage("platform");

if (!isBot) {
    firebase.initializeApp(firebaseConfig);

    window.auth = firebase.auth();

    window.auth.setPersistence(firebase.auth.Auth.Persistence.LOCAL);

    window.auth.onAuthStateChanged(async (user) => {
        await AuthStateChanged(user);
    });

    window.auth.getRedirectResult()
        .then((result) => {
            showToast(`getRedirectResult: ${result.credential?.idToken}`);
        }).catch((error) => {
            showError(error.message);
            sendLog(error);
        });

    if (!nativePlatforms.includes(platform)) {
        window.messaging = firebase.messaging();

        window.messaging.onMessage((payload) => {
            if (Notification.permission === 'granted') {
                const { title, body } = payload.data || {};
                new Notification(title, { body });
            }
        });
    }
}

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

            let usePopup = false;
            if (isLocalhost) usePopup = true;
            if (platform == "ios") usePopup = true;
            if (platform == "ios" && providerName == "google") usePopup = true;

            if (usePopup) {
                await window.auth.signInWithPopup(provider);
            }
            else {
                await window.auth.signInWithRedirect(provider);
            }
        } catch (error) {
            sendLog(error);
            throw new Error(error.message);
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
    }
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

    const token = await window.messaging.getToken({
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

async function FirebaseSignIn(provider) {
    if (typeof firebaseAuth === "undefined" || !firebaseAuth) {
        showError("Login is temporarily unavailable. Please make sure you have a stable connection or try again later.");

        if (typeof firebase === "undefined" || !firebase) {
            showError("firebase is undefined in initFirebase");
            sendLog("firebase is undefined in initFirebase");
        }
        else if (typeof firebaseAuth === "undefined") {
            showError("firebaseAuth is undefined in FirebaseSignIn");
            sendLog("firebaseAuth is undefined in FirebaseSignIn");
        }
        else if (!firebaseAuth) {
            showError("The login system is still loading. Please try again.");
        }

        return;
    }

    await firebaseAuth.signIn(provider);
}