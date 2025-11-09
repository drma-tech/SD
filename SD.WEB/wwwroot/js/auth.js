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

if (!firebase.apps.length) {
    firebase.initializeApp(firebaseConfig);
}

const auth = firebase.auth();

auth.getRedirectResult()
    .then((result) => {
        console.log('=== REDIRECT RESULT DEBUG ===');
        console.log('User:', result.user);
        console.log('Credential:', result.credential);
        console.log('OperationType:', result.operationType);
        console.log('AdditionalUserInfo:', result.additionalUserInfo);
        console.log('=== END DEBUG ===');

        if (result.user) {
            // ✅ Login via redirect BEM-SUCEDIDO
            return result.user.getIdToken().then(token => {
                DotNet.invokeMethodAsync("SD.WEB", "AuthChanged", token);
                showToast('Login successful via redirect!');
            });
        } else {
            // ❌ Nenhum usuário no redirect result
            console.log('No user in redirect result');
            // Verifica se já tem um usuário logado (caso normal)
            if (auth.currentUser) {
                return auth.currentUser.getIdToken().then(token => {
                    DotNet.invokeMethodAsync("SD.WEB", "AuthChanged", token);
                });
            } else {
                DotNet.invokeMethodAsync("SD.WEB", "AuthChanged", null);
            }
        }
    })
    .catch((error) => {
        console.error('Redirect result error:', error);
        // Em caso de erro, verifica se já tem usuário logado
        if (auth.currentUser) {
            auth.currentUser.getIdToken().then(token => {
                DotNet.invokeMethodAsync("SD.WEB", "AuthChanged", token);
            });
        } else {
            DotNet.invokeMethodAsync("SD.WEB", "AuthChanged", null);
        }
    });

auth.onAuthStateChanged(async (user) => {
    const token = user ? await user.getIdToken() : null;
    DotNet.invokeMethodAsync("SD.WEB", "AuthChanged", token);
    showToast('onAuthStateChanged');
    showToast(`token:${token}`);
});

window.firebaseAuth = {
    signIn: async () => {
        const provider = new firebase.auth.GoogleAuthProvider();

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