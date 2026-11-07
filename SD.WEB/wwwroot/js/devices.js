"use strict";

const { ATTConsent } = window.WTN || {};

export const ios = {
    checkATTConsent() {
        // TODO: If the iOS app's actual data flows meet Apple's cross-company tracking definition, call ATTConsent.request before enabling that tracking and respect a denied status. Do not request ATT for ordinary first-party analytics or unrelated processing. Verify the WebToNative dashboard's automatic ATT-on-launch setting because it is not represented in this repository.
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