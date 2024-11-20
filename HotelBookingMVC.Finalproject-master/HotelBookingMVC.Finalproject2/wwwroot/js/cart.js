$(document).ready(function () {
    // Update quantity
    $('.cart-item-quantity').change(function () {
        var itemId = $(this).data('cart-item-id');
        var quantity = $(this).val();

        $.post('/Cart/UpdateCart', { cartItemId: itemId, quantity: quantity })
            .done(function (response) {
                if (response.success) {
                    location.reload(); // Refresh the page to reflect changes
                } else {
                    alert(response.message);
                }
            });
    });

    // Remove item
    $('.remove-item').click(function () {
        var itemId = $(this).data('cart-item-id');

        $.post('/Cart/RemoveFromCart', { cartItemId: itemId })
            .done(function (response) {
                if (response.success) {
                    location.reload(); // Refresh the page to reflect changes
                } else {
                    alert(response.message);
                }
            });
    });
});
