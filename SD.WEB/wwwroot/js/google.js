"use strict";

function openGoogleCheckout(productId, type) {
    try {
        alert("before test");
        if (!window.WTN) alert("WTN plugin not found");
        alert("calling inAppPurchase");
        window.WTN.inAppPurchase({
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
        alert(`error: ${JSON.stringify(e)}`);
    }
}