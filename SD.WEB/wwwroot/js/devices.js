"use strict";

const { ATTConsent } = window.WTN || {};

export const ios = {
    checkATTConsent() {
        ATTConsent.status({
            callback: function (result) {
                if (result.granted) {
                    //Permission Granted or ios version 14.4 or lower
                } else {
                    //Permission Denied / not Determined due to some restrictions / not asked
                }
            },
        });
    },
};
