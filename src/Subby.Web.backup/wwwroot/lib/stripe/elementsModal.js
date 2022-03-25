$(document).ready(function () {
    $.ajax({
        type: "GET",
        url: "/api/stripe/public-key",
        headers: {
            "Content-Type": "application/json"
        },
        success: function (data) {
            setUpPaymentForm(data);
        },
        failure: function (response) {
            console.log(response);
        },
        error: function (response) {

        }
    });

    function setUpPaymentForm(data) {
        var stripe = Stripe(data.publicKey);
        var elements = stripe.elements();

        var style = {
            base: {
                iconColor: '#666EE8',
                color: '#31325F',
                lineHeight: '40px',
                fontWeight: 300,
                fontSize: '16px',

                '::placeholder': {
                    color: '#CFD7E0',
                },
            },
        };

        var cardNumberElement = elements.create('cardNumber', {
            style: style
        });
        cardNumberElement.mount('#card-number-element');

        var cardExpiryElement = elements.create('cardExpiry', {
            style: style
        });
        cardExpiryElement.mount('#card-expiry-element');

        var cardCvcElement = elements.create('cardCvc', {
            style: style
        });
        cardCvcElement.mount('#card-cvc-element');


        function setOutcome(result) {
            var errorElement = document.querySelector('.card-errors');
            errorElement.textContent = "";
            errorElement.classList.remove('visible');
            var submitBtn = document.getElementById('pay-button');
            var paymentPlan = $('input[type=radio][name=paymentPlan]:checked').val();
            if (result.token) {
                const token = result.token.id;
                $.ajax({
                    type: "POST",
                    url: "/api/stripe/capture",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    data: JSON.stringify({
                        token: token,
                        plan: paymentPlan
                    }),
                    success: function (data) {
                        $("#payment-modal").modal("hide");
                        window.location.href = "/payments/complete";
                    },
                    failure: function (response) {
                        console.log(response);
                    },
                    error: function (response) {
                        var displayError = document.getElementById("card-errors");
                        displayError.textContent = response.responseText;
                        // submitBtn.classList.remove('is-loading');
                    }
                });
            } else if (result.error) {
                errorElement.textContent = result.error.message;
                errorElement.classList.add('visible');
                submitBtn.classList.remove('is-loading');
            }
        }

        cardNumberElement.on('change', function (event) {
            setOutcome(event);
        });

        cardExpiryElement.on('change', function (event) {
            setOutcome(event);
        });

        cardCvcElement.on('change', function (event) {
            setOutcome(event);
        });

        $('input[type=radio][name=paymentPlan]').change(function () {
            var selectedPrice = data.plans.find(x => x.key == this.value).amount;
            $("#payment-price").text("£" + parseFloat(selectedPrice).toFixed(2));
            console.log(selectedPrice);
        });

        function makePayment(clientSecret, card) {
            var paymentPlan = $('input[type=radio][name=paymentPlan]:checked').val();
            var submitBtn = document.getElementById('pay-button');
            stripe.confirmCardPayment(clientSecret, {
                payment_method: {
                    card: card
                }
            })
            .then(function (result) {              
                if (result.error) {
                    // Show error to your customer
                    var displayError = document.getElementById("card-errors");
                    displayError.textContent = result.error.message;
                    submitBtn.classList.remove('is-loading');
                } else {
                    console.log(result);
                  $.ajax({
                    type: "POST",
                    url: "/api/stripe/capture",
                    headers: {
                      "Content-Type": "application/json"
                    },
                    data: JSON.stringify({
                      token: result.paymentIntent.id,
                      plan: paymentPlan
                    }),
                    success: function (data) {
                      $("#payment-modal").modal("hide");
                      window.location.href = "/payments/complete";
                    },
                    failure: function (response) {
                      console.log(response);
                    },
                    error: function (response) {
                        var displayError = document.getElementById("card-errors");
                        displayError.textContent = response.responseText;
                        submitBtn.classList.remove('is-loading');
                      // submitBtn.classList.remove('is-loading');
                    }
                  });
                }
            });
        }

        document.querySelector('#payment-form').addEventListener('submit', function (e) {
            e.preventDefault();
            var submitBtn = document.getElementById('pay-button');
            submitBtn.classList.add('is-loading');
            var options = {};

            var paymentPlan = $('input[type=radio][name=paymentPlan]:checked').val();
            $.ajax({
                type: "POST",
                url: "/api/stripe/payment-intents",
                headers: {
                    "Content-Type": "application/json"
                },
                data: JSON.stringify({
                    plan: paymentPlan
                }),
                success: function (data) {
                  makePayment(data.clientSecret, cardNumberElement);
                },
                failure: function (response) {
                    console.log(response);
                },
                error: function (response) {
                    var displayError = document.getElementById("card-errors");
                    displayError.textContent = response.responseText;
                    // submitBtn.classList.remove('is-loading');
                }
            });

            // stripe.createToken(cardNumberElement, options).then(setOutcome);
        });
    }
});
