"use strict";

async function startPaddle(token) {
    if (window.location.hostname == "localhost" || window.location.hostname.includes("dev.")) {
        Paddle.Environment.set("sandbox");
    }
    await Paddle.Initialize({
        token: token,
        eventCallback: function (data) {
            if (data.name == "checkout.completed") {
                const client = {
                    CustomerId: data.data.customer.id,
                    AddressId: data.data.customer.address.id,
                    ProductId: data.data.items[0].product.id
                };
                Paddle.Checkout.close();
                DotNet.invokeMethodAsync('SD.WEB', 'RegistrationSuccessful', client);
            }
        }
    });
}

async function getPlans(priceStandardMonth, priceStandardYear, pricePremiumMonth, pricePremiumYear) {
    const request = {
        items: [
            { quantity: 1, priceId: priceStandardMonth },
            { quantity: 1, priceId: priceStandardYear },
            { quantity: 1, priceId: pricePremiumMonth },
            { quantity: 1, priceId: pricePremiumYear }
        ]
    };

    const list = [];

    await Paddle.PricePreview(request)
        .then((result) => {
            for (let item of result.data.details.lineItems) {
                list.push({
                    product: Number(item.product.customData['enum']),
                    cycle: Number(item.price.customData['enum']),
                    price: item.formattedTotals.subtotal,
                    productId: item.product.id,
                    priceId: item.price.id
                });
            }
        })
        .catch((error) => {
            showError(error.error.detail);
        });

    return list;
}

function openCheckout(priceId, email, locale, customerId, addressId, transaction_id) {
    let customer;
    if (customerId) {
        customer = {
            id: customerId,
            address: {
                id: addressId
            }
        }
    }
    else if (email) {
        customer = {
            email: email
        }
    }

    Paddle.Checkout.open({
        settings: {
            displayMode: "overlay",
            theme: "light",
            locale: locale,
            showAddDiscounts: false,
            showAddTaxId: false
        },
        items: [
            {
                priceId: priceId,
                quantity: 1
            }
        ],
        customer: customer,
        transaction_id: transaction_id
    });
}