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
        .catch(error => showError(error.message));
}

function GetLocalStorage(key) {
    return window.localStorage.getItem(key);
}

function SetLocalStorage(key, value) {
    return window.localStorage.setItem(key, value);
}

function TryDetectPlatform() {
    if (GetLocalStorage('platform')) return; //if populate before, cancel, cause detection (windows) only works for first call

    let isWindows = document.referrer == "app-info://platform/microsoft-store";
    let isGooglePlay = document.referrer?.includes("android-app://"); //let isAndroid = /(android)/i.test(navigator.userAgent);
    let isIOS = document.cookie.split('; ').some(cookie => cookie === 'app-platform=iOS App Store');

    if (isWindows)
        SetLocalStorage('platform', 'windows');
    else if (isGooglePlay)
        SetLocalStorage('platform', 'play');
    else if (isIOS)
        SetLocalStorage('platform', 'ios');
    else
        SetLocalStorage('platform', 'webapp');
}

async function getUserInfo() {
    try {
        if (!window.location.host.includes('localhost')) {
            const response = await fetch('/.auth/me');
            if (response.ok) {
                const userInfo = await response.json();
                if (userInfo?.clientPrincipal?.userId) {
                    return userInfo.clientPrincipal.userId;
                }
                return null;
            }
            else {
                return null;
            }
        }
        else {
            return null;
        }
    } catch (error) {
        showError(error.message);
        return null;
    }
}

function showError(message) {
    if (window.DotNet)
        try {
            DotNet.invokeMethodAsync('SD.WEB', 'ShowError', message);
        }
        catch {
            showToast(message);
        }
    else {
        showToast(message);
    }
}

function showToast(message) {
    const container = document.getElementById('error-container');
    if (!container) return;

    container.textContent = message;
    container.style.display = 'block';

    setTimeout(() => {
        container.style.display = 'none';
    }, 5000);
}

function changeDarkMode() {
    let theme = GetLocalStorage('theme');
    theme = (theme == "light" ? "dark" : "light");
    SetLocalStorage('theme', theme);

    document.body.setAttribute("data-bs-theme", theme);
}

function animationShake(cssClass) {
    const alertBoxes = document.querySelectorAll(cssClass);

    alertBoxes.forEach(el => {
        el.classList.add("shake");
        setTimeout(() => el.classList.remove("shake"), 300);
    });

    if (navigator.vibrate) { navigator.vibrate(200); }
}

function animationBlink(cssClass) {
    const alertBoxes = document.querySelectorAll(cssClass);

    alertBoxes.forEach(el => {
        el.classList.add("blink");
        setTimeout(() => { el.classList.remove("blink"); }, 1500);
    });
}