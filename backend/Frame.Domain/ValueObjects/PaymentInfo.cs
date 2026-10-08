using Frame.Domain.Common;

namespace Frame.Domain.ValueObjects;

/// <summary>
/// What we keep from a successful payment. Only safe data:
/// the gateway reference, the card brand and the last 4 digits.
/// The full card number and CVV never reach the server.
/// </summary>
public sealed record PaymentInfo
{
    /// <summary>The payment id returned by the gateway, used later for refunds.</summary>
    public string Reference { get; }

    /// <summary>e.g. "Visa" or "Mastercard", shown on the invoice.</summary>
    public string CardBrand { get; }

    /// <summary>Exactly 4 digits, e.g. "4242", shown as **** 4242.</summary>
    public string CardLast4 { get; }

    /// <summary>When the payment succeeded (UTC).</summary>
    public DateTime PaidAt { get; }

    public PaymentInfo(string reference, string cardBrand, string cardLast4, DateTime paidAt)
    {
        Reference = Guard.NotEmpty(reference, "PAYMENT_REFERENCE_REQUIRED");
        CardBrand = Guard.NotEmpty(cardBrand, "CARD_BRAND_REQUIRED");

        var last4 = Guard.NotEmpty(cardLast4, "CARD_LAST4_INVALID");
        if (last4.Length != 4 || !last4.All(char.IsDigit))
            throw new DomainException("CARD_LAST4_INVALID");

        CardLast4 = last4;
        PaidAt = paidAt;
    }
}
