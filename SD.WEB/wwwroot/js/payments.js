"use strict";

import { baseApiUrl } from "./main.js";
import { storage, notification, interop } from "./utils.js";

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

export const stripe = {
    async openCheckout(priceId) {
        try {
            const auth = storage.getLocalStorage("auth");
            let response;

            if (auth === "firebase") {
                const token = await window.firebase.currentUser.getIdToken();

                response = await fetch(
                    `${baseApiUrl}/api/stripe/create-checkout-session/${priceId}?url=${window.location.href}`,
                    {
                        method: "POST",
                        headers: {
                            "X-Firebase-Token": `Bearer ${token}`,
                        },
                    }
                );
            } else if (auth === "supabase") {
                const { data } = await window.supabase.auth.getSession();

                response = await fetch(
                    `${baseApiUrl}/api/stripe/create-checkout-session/${priceId}?url=${window.location.href}`,
                    {
                        method: "POST",
                        headers: {
                            "X-Supabase-Token": `Bearer ${data.session.access_token}`,
                        },
                    }
                );
            }

            const checkoutUrl = await response.text();

            window.location.href = checkoutUrl;
        } catch (e) {
            notification.showError(`error: ${JSON.stringify(e)}`);
        }
    },
};