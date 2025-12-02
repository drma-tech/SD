"use strict";

async function startPaddle(token) {
    try {
        if (isLocalhost || isDev) {
            Paddle.Environment.set("sandbox");
        }
        await Paddle.Initialize({
            token: token,
            eventCallback: function (data) {
                if (data.name == "checkout.completed") {
                    //Wait for my API to be called and update the subscription.
                    setTimeout(() => {
                        Paddle.Checkout.close();
                        invokeDotNetWhenReady("SD.WEB", "RegistrationSuccessful");
                    }, 1000);
                }
            },
        });
    } catch (error) {
        sendLog(error);
        showError(error.message);
    }
}

function openCheckout(priceId, email, locale, customerId) {
    try {
        let customer;
        if (customerId) {
            customer = {
                id: customerId,
            };
        } else if (email) {
            customer = {
                email: email,
            };
        }

        let isDark = GetLocalStorage("dark-mode") == "true";

        Paddle.Checkout.open({
            settings: {
                displayMode: "overlay",
                theme: isDark ? "dark" : "light",
                locale: locale,
                showAddDiscounts: false,
                showAddTaxId: false,
            },
            items: [
                {
                    priceId: priceId,
                    quantity: 1,
                },
            ],
            customer: customer,
        });
    } catch (error) {
        sendLog(error);
        showError(error.message);
    }
}