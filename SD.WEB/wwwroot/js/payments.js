"use strict";

import { storage, notification, interop } from "./utils.js";

export const apple = {
    openCheckout(productId) {
        if (!window.appConfig.isWebview) {
            notification.showError("It looks like you're accessing accessing this from a browser, but this feature is only available in the app. Please open the app to continue.");
            Sentry.captureMessage("It looks like you're accessing accessing this from a browser, but this feature is only available in the app. Please open the app to continue.", "error");
            return;
        }

        window.WTN.inAppPurchase({
            productId: productId,
            callback: function (data) {
                if (!data) {
                    notification.showError("No data returned from purchase");
                    Sentry.captureMessage("No data returned from purchase", "error");
                    return;
                }

                if (!data.isSuccess) {
                    notification.showError("Purchase failed or canceled");
                    Sentry.captureMessage("Purchase failed or canceled", "error");
                    return;
                }

                const receiptData = data.receiptData;

                if (!receiptData) {
                    notification.showError("Receipt not found");
                    Sentry.captureMessage("Receipt not found", "error");
                    return;
                }

                interop.invokeDotNetWhenReady(
                    "SD.WEB",
                    "AppleVerify",
                    receiptData
                );
            },
        });
    },
    getReceiptData() {
        if (!window.appConfig.isWebview) {
            notification.showError("It looks like you're accessing accessing this from a browser, but this feature is only available in the app. Please open the app to continue.");
            Sentry.captureMessage("It looks like you're accessing accessing this from a browser, but this feature is only available in the app. Please open the app to continue.", "error");
            return;
        }

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
            if (!window.appConfig.isWebview) {
                notification.showError("It looks like you're accessing accessing this from a browser, but this feature is only available in the app. Please open the app to continue.");
                Sentry.captureMessage("It looks like you're accessing accessing this from a browser, but this feature is only available in the app. Please open the app to continue.", "error");
                return;
            }

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
            Sentry.captureException(e);
        }
    },
};

export const stripe = {
    async openCheckout(priceId) {
        try {
            const auth = storage.getLocalStorage("auth");
            let response;

            if (auth === "clerk") {
                const session = window.clerk.session;
                const token = session ? await session.getToken() : null;

                if (!token) {
                    notification.showError("Failed to retrieve authentication token.");
                    Sentry.captureMessage("Failed to retrieve authentication token.", "error");
                    return;
                }

                const url = encodeURIComponent(window.location.href);

                response = await fetch(
                    `${window.appConfig.baseApiUrl}/api/stripe/create-checkout-session/${priceId}?url=${url}`,
                    {
                        method: "POST",
                        headers: {
                            "X-Clerk-Token": `Bearer ${token}`,
                            "X-App-Version": window.appVersion,
                        },
                    }
                );
            }

            if (!response.ok) {
                const error = await response.text();
                notification.showError(error);
                Sentry.captureMessage(error, "error");
                return;
            }

            const checkoutUrl = await response.text();

            window.location.href = checkoutUrl;
        } catch (e) {
            notification.showError(`error: ${JSON.stringify(e)}`);
            Sentry.captureException(e);
        }
    },
};