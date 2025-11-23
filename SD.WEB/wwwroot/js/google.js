"use strict";

function openGoogleCheckout(productId, type) {
    try {
        if (!WTN) alert("WTN plugin not found");
        WTN.inAppPurchase({
            productId: productId,
            productType: type,
            isConsumable: true,
            callback: function (data) {
                var receiptData = data.receiptData; //save on cosmos (Client.AuthPayment)
                showToast(JSON.stringify(receiptData));
                showToast(JSON.stringify(data));
                if (data.isSuccess) {
                   
                }
            }
        })
    } catch (e) {
        showError(`error: ${JSON.stringify(e)}`);
    }
}