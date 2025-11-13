"use strict";

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

auth.onAuthStateChanged(async (user) => {
    const token = user ? await user.getIdToken() : null;
    await invokeDotNetWhenReady("SD.WEB", "AuthChanged", token);
});

window.firebaseAuth = {
    signIn: async (providerName, email) => {
        let provider;

        switch (providerName) {
            case "google": {
                provider = new firebase.auth.GoogleAuthProvider();
                break;
            }
            case "apple": {
                provider = new firebase.auth.OAuthProvider("apple.com");
                break;
            }
            case "facebook": {
                provider = new firebase.auth.FacebookAuthProvider();
                break;
            }
            case "microsoft":
                provider = new firebase.auth.OAuthProvider("microsoft.com");
                break;

            case "yahoo":
                provider = new firebase.auth.OAuthProvider("yahoo.com");
                break;

            case "x":
                provider = new firebase.auth.TwitterAuthProvider();
                break;
            default:
                {
                    throw new Error(`Unsupported provider: ${providerName}`);
                }
        }

        await auth.signInWithPopup(provider);
    },
    signOut: async () => {
        await auth.signOut();
    }
};