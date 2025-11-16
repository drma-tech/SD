"use strict";

// =========================
//  FIREBASE INIT
// =========================

const firebaseConfig = {
    apiKey: "AIzaSyDj5LpsT7-bra4hvuvb5E_BPSlD7Wr29nQ",
    authDomain: "streaming-discovery-4c483.firebaseapp.com",
    projectId: "streaming-discovery-4c483",
    storageBucket: "streaming-discovery-4c483.firebasestorage.app",
    messagingSenderId: "394152837411",
    appId: "1:394152837411:web:06b7216d2859bdc1224c96",
    measurementId: "G-JKXTVXM0EX"
};

firebase.initializeApp(firebaseConfig);

const auth = firebase.auth();
const messaging = firebase.messaging();

auth.setPersistence(firebase.auth.Auth.Persistence.LOCAL);

// =========================
//  AUTH HANDLERS
// =========================

auth.onAuthStateChanged(async (user) => {
    const token = user ? await user.getIdToken() : null;
    showToast(token);
    await invokeDotNetWhenReady("SD.WEB", "AuthChanged", token);
});

window.firebaseAuth = {
    signIn: async (providerName, email) => {
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

        try {
            await auth.signInWithRedirect(provider);
        } catch (error) {
            throw new Error(error.message);
        }
    },

    signOut: async () => {
        try {
            await auth.signOut();
        } catch (error) {
            throw new Error(error.message);
        }
    }
};

auth.getRedirectResult()
    .then((result) => {
        if (result.credential) {
            let credential = result.credential;
            let token = credential.accessToken;
            showToast(token);
            await invokeDotNetWhenReady("SD.WEB", "AuthChanged", token);
        }
    });

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