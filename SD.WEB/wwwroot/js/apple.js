"use strict";

function openAppleCheckout(productId) {
    try {
        if (!window.WTN) showError("WTN plugin not found");
        alert("calling inAppPurchase");
        window.WTN.inAppPurchase({
            productId: productId,
            callback: function (data) {
                var receiptData = data.receiptData; //save on cosmos (Client.AuthPayment)
                showToast(receiptData);
                showToast(data);
                if (data.isSuccess) {
                    // use this receipt data to verify transaction from app store
                    // refer : https://developer.apple.com/documentation/appstorereceipts/verifyreceipt

                    //{
                    //    "receipt-data": "xxxxxxxxxxxxxxx",
                    //        "password": "shared secret from iTunes connect",
                    //            "exclude-old-transactions": true
                    //}

                    // https://developer.apple.com/documentation/appstorereceipts/status

                    // https://sandbox.itunes.apple.com/verifyReceipt
                    // https://buy.itunes.apple.com/verifyReceipt
                }
            }
        })
    } catch (e) {
        alert(`error: ${JSON.stringify(e) }`);
    }
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