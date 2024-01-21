async function startPaddle(token) {
    if (window.location.hostname == "localhost" || window.location.hostname.includes("dev.")) {
        Paddle.Environment.set("sandbox");
    }
    await Paddle.Setup({
        token: token,
        eventCallback: function (data) {
            if (data.name == "checkout.completed") {
                let client = {
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

async function getPlans(price_standard_month, price_standard_year, price_premium_month, price_premium_year) {
    let request = {
        items: [
            { quantity: 1, priceId: price_standard_month },
            { quantity: 1, priceId: price_standard_year },
            { quantity: 1, priceId: price_premium_month },
            { quantity: 1, priceId: price_premium_year }
        ]
    }

    let list = [];

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
            console.error(error);
            DotNet.invokeMethodAsync('SD.WEB', 'ShowError', error.error.detail);
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