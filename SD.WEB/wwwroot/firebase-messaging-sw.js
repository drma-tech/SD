importScripts(
    'https://www.gstatic.com/firebasejs/12.6.0/firebase-app-compat.js',
    'https://www.gstatic.com/firebasejs/12.6.0/firebase-messaging-compat.js'
);

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

const messaging = firebase.messaging();

messaging.onBackgroundMessage((payload) => {
    console.log('Received background message ', payload);

    //const { title, body, icon } = payload.notification || {};
    //self.registration.showNotification(title, { body, icon });
});