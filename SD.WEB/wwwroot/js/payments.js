"use strict";

import { isLocalhost, isDev } from "./main.js";
import { storage, notification, interop } from "./utils.js";

export const paddle = {
    async start(token) {
        try {
            if (isLocalhost || isDev) {
                window.Paddle.Environment.set("sandbox");
            }
            await window.Paddle.Initialize({
                token: token,
                eventCallback: function (data) {
                    if (data.name === "checkout.completed") {
                        //Wait for my API to be called and update the subscription.
                        setTimeout(() => {
                            window.Paddle.Checkout.close();
                            interop.invokeDotNetWhenReady(
                                "SD.WEB",
                                "RegistrationSuccessful"
                            );
                        }, 1000);
                    }
                },
            });
        } catch (error) {
            notification.sendLog(error);
            notification.showError(error.message);
        }
    },
    openCheckout(priceId, email, locale, customerId) {
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

            let isDark = storage.getLocalStorage("dark-mode") === "true";

            window.Paddle.Checkout.open({
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
            notification.sendLog(error);
            notification.showError(error.message);
        }
    },
};

export const apple = {
    openCheckout(productId) {
        window.WTN.inAppPurchase({
            productId: productId,
            callback: function (data) {
                if (data.isSuccess) {
                    if (!data) {
                        notification.showToast(
                            "No data returned from purchase"
                        );
                        return;
                    }
                    if (!data.isSuccess) {
                        notification.showToast("Purchase failed or canceled");
                        return;
                    }

                    const receiptData = data.receiptData;
                    if (!receiptData) {
                        notification.showToast("Receipt not found");
                        return;
                    }

                    interop.invokeDotNetWhenReady(
                        "SD.WEB",
                        "AppleVerify",
                        receiptData
                    );
                }
            },
        });
    },
    getReceiptData() {
        window.WTN.getReceiptData({
            callback: function (data) {
                if (data.receiptData.isSuccess) {
                    // use this receipt data to verify transaction from app store
                    // refer : https://developer.apple.com/documentation/appstorereceipts/verifyreceipt
                }
            },
        });
    },
};

export const google = {
    openCheckout(productId, type) {
        try {
            window.WTN.inAppPurchase({
                productId: productId,
                productType: type,
                isConsumable: true,
                callback: function (data) {
                    let receiptData = data.receiptData; //save on cosmos (Client.AuthPayment)
                    notification.showToast(JSON.stringify(receiptData));
                    notification.showToast(JSON.stringify(data));
                    if (data.isSuccess) {
                        //do something when purchase is successful
                    }
                },
            });
        } catch (e) {
            notification.showError(`error: ${JSON.stringify(e)}`);
        }
    },
};
