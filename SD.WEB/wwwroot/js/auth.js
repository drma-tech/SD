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
const provider = new firebase.auth.GoogleAuthProvider();

auth.onAuthStateChanged(async (user) => {
    let token = user ? await user.getIdToken() : null;

    if (token == null) {
        const result = await auth.getRedirectResult();
        if (result?.user) {
            token = await result.user.getIdToken();
        }
    }

    DotNet.invokeMethodAsync("SD.WEB", "AuthChanged", token);
});

window.firebaseAuth = {
    signIn: async () => {
        if (window.location.hostname === "localhost") {
            await auth.signInWithPopup(provider);
        }
        else {
            await auth.signInWithRedirect(provider);
        }
    },
    signOut: async () => {
        await auth.signOut();
    }
};