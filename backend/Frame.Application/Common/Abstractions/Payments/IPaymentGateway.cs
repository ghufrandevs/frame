namespace Frame.Application.Common.Abstractions.Payments;

/// <summary>
/// Charges and refunds a customer's card through a payment provider.
/// The server only ever receives a token from the provider, never the card
/// number or CVV. Swap the implementation (fake today, real provider later)
/// in DependencyInjection without touching the booking service.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Charges the amount computed by the server. Returns a failed result
    /// (not an exception) when the card is declined.
    /// </summary>
    Task<PaymentResult> ChargeAsync(string paymentToken, decimal amount, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the amount of an earlier charge, identified by its reference.
    /// Used when a slot is lost after payment, and when an admin cancels.
    /// </summary>
    Task<bool> RefundAsync(string paymentReference, decimal amount, CancellationToken cancellationToken);
}