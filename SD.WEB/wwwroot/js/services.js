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
                    const name = user?.claims?.find(c => c.typ === "name")?.val;
                    const email = user?.claims?.find(c => c.typ === "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.val;
                    Userback.identify(user?.userId, {
                        name: name,
                        email: email
                    });
                }
            })
            .catch(error => {
                showError(error.message);
            });
    };
}

// adsense
window.adsenseManager = {
    isLoaded: false,
    observers: new Map(),

    load: function (client) {
        return new Promise((resolve, reject) => {
            if (window.adsenseManager.isLoaded) return resolve();

            if (document.querySelector(`script[src*="pagead2.googlesyndication.com"]`)) {
                window.adsenseManager.isLoaded = true;
                return resolve();
            }

            const s = document.createElement("script");
            s.async = true;
            s.src = `https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js?client=${client}`;
            s.crossOrigin = "anonymous";
            s.onload = () => {
                window.adsenseManager.isLoaded = true;
                resolve();
            };
            s.onerror = reject;
            document.head.appendChild(s);
        });
    },

    observeAd: function (adId) {
        const adElement = document.getElementById(adId);
        if (!adElement) return;

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.pushAd(adId);
                    observer.unobserve(adElement);
                    this.observers.delete(adId);
                }
            });
        }, { threshold: 0.1 });

        observer.observe(adElement);
        this.observers.set(adId, observer);
    },

    pushAd: function (adId) {
        (adsbygoogle = window.adsbygoogle || []).push({});
    },

    removeObserver: function (adId) {
        if (this.observers.has(adId)) {
            const observer = this.observers.get(adId);
            observer.disconnect();
            this.observers.delete(adId);
        }
    },
};

window.loadAdById = function (adId) {
    const checkAds = setInterval(() => {
        const ins = document.getElementById(adId);

        if (ins && !ins.hasAttribute("data-adsbygoogle-status") && window.adsbygoogle) {
            (adsbygoogle = window.adsbygoogle || []).push({});
            console.log("AdSense ad requested for", adId);
            clearInterval(checkAds);
        }
    }, 200);
};

window.removeAdById = function (adId) {
    const el = document.getElementById(adId);
    if (el && el.parentNode) {
        el.parentNode.removeChild(el);
    }
};