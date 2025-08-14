"use strict";

// Google Analytics
window.initGoogleAnalytics = function (code) {
    if (!window.location.host.includes("localhost") && GetLocalStorage("platform") !== "ios") {
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
    }
}

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
}

// Disable robots for dev environment
window.setRobotsMeta = function () {
    if (window.location.hostname.includes("dev")) {
        const meta = document.createElement("meta");
        meta.name = "robots";
        meta.content = "noindex, nofollow";
        document.head.appendChild(meta);
    }
}

// userback
window.initUserBack = function () {
    window.Userback = window.Userback || {};
    Userback.access_token = "A-A2J4M5NKCbDp1QyQe7ogemmmq";
    (function (d) {
        var s = d.createElement('script'); s.async = true; s.src = 'https://static.userback.io/widget/v1.js'; (d.head || d.body).appendChild(s);
    })(document);
    const browserLang = navigator.language || navigator.userLanguage;
    Userback.widget_settings = {
        language: GetLocalStorage("language") ?? browserLang.slice(0, 2),
        logo: window.location.origin + "/icon/icon-71.png"
    };
    Userback.on_load = () => {
        getUserInfo()
            .then(user => {
                if (user) {
                    const email = user?.userDetails;
                    Userback.identify(user?.userId, {
                        name: email?.split("@")[0],
                        email: email
                    });
                }
            })
            .catch(error => {
                showError(error.message);
            });
    };
}

window.initUsetiful = function () {
    getUserInfo()
        .then(user => {
            if (user) {
                window.usetifulTags = {
                    userId: user?.userId
                };

                (function (w, d, s) {
                    var a = d.getElementsByTagName('head')[0];
                    var r = d.createElement('script');
                    r.async = 1;
                    r.src = s;
                    r.setAttribute('id', 'usetifulScript');
                    r.dataset.token = "7a15ef5f4b9e80c2902d622ee3dd32be";  // This is your unique USETIFUL token. Don’t change it.
                    a.appendChild(r);
                })(window, document, "https://www.usetiful.com/dist/usetiful.js");
            }
        })
        .catch(error => {
            showError(error.message);
        });
}