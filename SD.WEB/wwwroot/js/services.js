"use strict";

// Google Analytics
window.initGoogleAnalytics = function (code) {
    window.dataLayer = window.dataLayer || [];
    function gtag() { dataLayer.push(arguments); }
    gtag("js", new Date());

    getUserInfo()
        .then(user => {
            if (user) {
                gtag("config", code, {
                    'user_id': user?.userId
                });
            }
            else {
                gtag("config", code);
            }
        })
        .catch(error => {
            showError(error.message);
        });
};

// Microsoft Clarity
window.initClarity = function (code) {
    if (!window.location.host.includes("localhost") && GetLocalStorage("platform") !== "ios") {
        (function (c, l, a, r, i, t, y) {
            c[a] = c[a] || function () { (c[a].q = c[a].q || []).push(arguments) };
            t = l.createElement(r); t.async = 1; t.src = "https://www.clarity.ms/tag/" + i;
            y = l.getElementsByTagName(r)[0]; y.parentNode.insertBefore(t, y);
        })(window, document, "clarity", "script", code);

        // Check if Clarity is loaded and call the consent function
        const clarityCheckInterval = setInterval(function () {
            if (window.clarity) {
                window.clarity("consent");
                clearInterval(clarityCheckInterval);
            }
        }, 5000);
    }
};

// Disable robots for dev environment
window.setRobotsMeta = function () {
    if (window.location.hostname.includes("dev")) {
        const meta = document.createElement("meta");
        meta.name = "robots";
        meta.content = "noindex, nofollow";
        document.head.appendChild(meta);
    }
};

// userback
window.initUserBack = function () {
    getUserInfo()
        .then(user => {
            if (user) {
                Userback.user_data = {
                    id: user?.userId,
                    info: {
                        name: user?.userDetails,
                        email: user?.userDetails
                    }
                };
            }
        })
        .catch(error => {
            showError(error.message);
        });
};
