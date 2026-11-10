import { notification } from "./utils.js";

//avoid google (and others) search console or possible bots
if (window.appConfig?.isBot === false && window.appConfig?.disableServiceWorker === false) {
    if ('serviceWorker' in navigator) {
        navigator.serviceWorker.register("service-worker.js", { updateViaCache: 'none' }).catch((err) => {
            notification.showError(
                "Offline mode couldn't be enabled. This may be due to a slow connection. Try refreshing the page or reopening the app."
            );
        });
    }
}