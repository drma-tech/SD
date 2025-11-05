"use strict";

function openAppleCheckout(productId) {
    //DotNet.invokeMethodAsync('SD.WEB', 'AppleVerify', null);
    if (!WTN) alert("WTN plugin not found");
    WTN.inAppPurchase({
        productId: productId,
        callback: function (data) {
            showToast(JSON.stringify(receiptData));
            if (data.isSuccess) {
                if (!data) { showToast("No data returned from purchase"); return; }
                if (!data.isSuccess) { showToast("Purchase failed or canceled"); return; }

                const receiptData = data.receiptData;
                if (!receiptData) { showToast("Receipt not found"); return; }

                DotNet.invokeMethodAsync('SD.WEB', 'AppleVerify', receiptData);
            }
        }
    })
}

function getReceiptData() {
    if (!window.WTN) showError("WTN plugin not found");
    window.WTN.getReceiptData({
        callback: function (data) {
            var receiptData = data.receiptData;
            if (data.isSuccess) {
                // use this receipt data to verify transaction from app store
                // refer : https://developer.apple.com/documentation/appstorereceipts/verifyreceipt
            }
        }
    })
}

function checkATTConsent() {
    if (!window.WTN) showError("WTN plugin not found");

    const { ATTConsent } = window.WTN

    ATTConsent.status({
        callback: function (result) {
            if (result.granted) {
                //Permission Granted or ios version 14.4 or lower
            } else {
                //Permission Denied / not Determined due to some restrictions / not asked
            }
        }
    })
}