"use strict";

async function startPaddle(token) {
    if (window.location.hostname == "localhost" || window.location.hostname.includes("dev.")) {
        Paddle.Environment.set("sandbox");
    }
    await Paddle.Initialize({
        token: token,
        eventCallback: function (data) {
            if (data.name == "checkout.completed") {
                Paddle.Checkout.close();
                DotNet.invokeMethodAsync('SD.WEB', 'RegistrationSuccessful');
            }
        }
    });
}

function openCheckout(priceId, email, locale, customerId) {
    let customer;
    if (customerId) {
        customer = {
            id: customerId
        }
    }
    else if (email) {
        customer = {
            email: email
        }
    }

    let isDark = GetLocalStorage("dark-mode") == "true";

    Paddle.Checkout.open({
        settings: {
            displayMode: "overlay",
            theme: isDark ? "dark" : "light",
            locale: locale,
            showAddDiscounts: false,
            showAddTaxId: false
        },
        items: [
            {
                priceId: priceId,
                quantity: 1
            }
        ],
        customer: customer
    });
}