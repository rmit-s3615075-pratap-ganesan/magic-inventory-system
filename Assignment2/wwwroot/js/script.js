// Create a Stripe client.



var stripe = Stripe('pk_test_R2D0YvT69N2vwso3oUXKbwlK');

// Create an instance of Elements.
var elements = stripe.elements();

// Custom styling can be passed to options when creating an Element.
// (Note that this demo uses a wider set of styles than the guide below.)
var style = {
  base: {
    color: '#32325d',
    lineHeight: '18px',
    fontFamily: '"Helvetica Neue", Helvetica, sans-serif',
    fontSmoothing: 'antialiased',
    fontSize: '16px',
    '::placeholder': {
      color: '#aab7c4'
    }
  },
  invalid: {
    color: '#fa755a',
    iconColor: '#fa755a'
  }
};

// Create an instance of the card Element.
var card = elements.create('card', {style: style});

// Add an instance of the card Element into the `card-element` <div>.
card.mount('#card-element');

// Handle real-time validation errors from the card Element.
card.addEventListener('change', function(event) {
  var displayError = document.getElementById('card-errors');
  if (event.error) {
    $('#danger-alert').show();
    displayError.textContent = event.error.message;
  
  } else {
    displayError.textContent = '';
    $('#danger-alert').hide();
  }
});

// Handle form submission.
var form = document.getElementById('payment-form');
form.addEventListener('submit', function(event) {
  event.preventDefault();

  stripe.createToken(card).then(function(result) {
    if (result.error) {
      // Inform the user if there was an error.
   
      errorElement.textContent = result.error.message;
        $("#danger-alert").alert();
      $("#danger-alert").fadeTo(2000, 500).slideUp(500, function(){
     $("#danger-alert").slideUp(500);
     });
    } 
    else {
      return window.location.href ="http://localhost:5000/OrderHistory/Create";
      // Send the token to your server.
      stripeTokenHandler(result.token);
    }
  });
});

