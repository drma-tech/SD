Paddle.Environment.set("sandbox");
Paddle.Setup({
    token: 'test_3af0345347aa7b0122eca72495e',
    eventCallback: function (data) {
        if (data.name == "checkout.completed") {
            let client = {
                CustomerId: data.data.customer.id,
                AddressId: data.data.customer.address.id,
                ProductId: data.data.items[0].product.id
            };
            DotNet.invokeMethodAsync('SD.WEB', 'RegistrationSuccessful', client)
        }
    }
});

let product_standard = "pro_01hk9578q9g4g82d91vxbz3zk5";
let product_premium = "pro_01hm2tdgses7t04zngyvxchd3r";

let price_standard_month = "pri_01hk96x0wdc6xrjrc8sfy39xj5";
let price_standard_year = "pri_01hk958vt3a5y9yc5dchwtgbp7";

let price_premium_month = "pri_01hm2tgkt6gxay2y3m4bygxfbe";
let price_premium_year = "pri_01hm2thqafj7bet8rkpsekxzq1";

async function getPlans() {
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
        });

    return list;
}

function openCheckout(priceId, email, locale, customerId, addressId) {
    let customer;
    if (customerId) {
        customer = {
            id: customerId,
            address: {
                id: addressId
            }
        }
    }
    else {
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
        customer: customer
    });
}