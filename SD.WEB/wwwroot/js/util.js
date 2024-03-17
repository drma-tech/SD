"use strict";

function getWindowDimensions() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

function share(url) {
    if (!("share" in navigator)) {
        alert('Web Share API not supported.');
        return;
    }

    navigator.share({ url: url })
        .then(() => console.log('Successful share'))
        .catch(error => console.log('Error sharing:', error));
}

function GetLocalStorage(key) {
    return window.localStorage.getItem(key);
}

function SetLocalStorage(key, value) {
    return window.localStorage.setItem(key, value);
}

function TryDetectWindowsStore() {
    let isWindows = document.referrer == "app-info://platform/microsoft-store";

    if (isWindows)
        SetLocalStorage('platform', 'windows');
    else
        SetLocalStorage('platform', 'webapp');
}