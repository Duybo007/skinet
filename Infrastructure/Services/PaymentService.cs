using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services;

public class PaymentService(IConfiguration config, ICartService cartService, IProductRepository productRepo, IGenericRepository<DeliveryMethod> dmRepo) : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        // Set the Stripe API key from configuration
        StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

        // Retrieve the shopping cart
        var cart = await cartService.GetCartAsync(cartId);

        if (cart == null) return null; // Return null if the cart does not exist

        var shippingPrice = 0m;

        // Check if a delivery method is selected
        if (cart.DeliveryMethodId.HasValue)
        {
            var DeliveryMethod = await dmRepo.GetByIdAsync((int)cart.DeliveryMethodId);

            if (DeliveryMethod == null) return null; // Return null if the delivery method is invalid

            shippingPrice = DeliveryMethod.Price;
        }

        // Validate and update item prices in the cart
        foreach (var item in cart.Items)
        {
            var productItem = await productRepo.GetProductByIdAsync(item.ProductId);

            if (productItem == null) return null; // Return null if the product does not exist

            // Ensure the customer is charged the correct price
            if (item.Price != productItem.Price)
            {
                item.Price = productItem.Price;
            }
        }

        var service = new PaymentIntentService(); // Create a Stripe payment intent service instance
        PaymentIntent? intent = null;

        if (string.IsNullOrEmpty(cart.PaymentIntentId)) // If no existing payment intent, create a new one
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100, // Calculate total amount in cents
                Currency = "usd",
                PaymentMethodTypes = ["card"] // Specify accepted payment method(s)
            };
            intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id;
            cart.ClientSecret = intent.ClientSecret; // Store the client secret for frontend use
        }
        else // If payment intent exists, update the amount
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(x => x.Quantity * (x.Price * 100)) + (long)shippingPrice * 100 // Recalculate total amount in cents
            };
            intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        }

        await cartService.SetCartAsync(cart); // Save updated cart details

        return cart; // Return the updated cart
    }
}
