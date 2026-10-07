using Frame.Application.Common.Abstractions.Payments;
using Frame.Application.Common.Errors;
using Microsoft.Extensions.Logging;

namespace Frame.Infrastructure.Payments;

/// <summary>
/// Test payment provider. The outcome is read from the token itself,
/// shaped "tok_{outcome}_{brand}_{last4}", e.g. tok_ok_visa_4242.
/// Outcomes: ok, declined, nofunds. Anything malformed fails safely.
/// Logs brand, last 4 and reference only: never the token or card data.
/// </summary>
internal sealed class FakePaymentGateway : IPaymentGateway
{
    private const string TokenPrefix = "tok";
    private const int TokenParts = 4;
    private const string ReferencePrefix = "PAY-";

    // Mimics a real provider's network time, so the UI loading state can be tested.
    private static readonly TimeSpan SimulatedLatency = TimeSpan.FromMilliseconds(500);

    private readonly ILogger<FakePaymentGateway> _logger;

    public FakePaymentGateway(ILogger<FakePaymentGateway> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> ChargeAsync(string paymentToken, decimal amount, CancellationToken cancellationToken)
    {
        await Task.Delay(SimulatedLatency, cancellationToken);

        if (amount <= 0 || !TryParseToken(paymentToken, out var outcome, out var brand, out var last4))
        {
            _logger.LogWarning("Payment failed: malformed token or invalid amount");
            return PaymentResult.Failure(ErrorCodes.PaymentFailed);
        }

        var result = outcome switch
        {
            "ok" => PaymentResult.Success(NewReference(), brand, last4),
            "declined" => PaymentResult.Failure(ErrorCodes.PaymentDeclined),
            "nofunds" => PaymentResult.Failure(ErrorCodes.InsufficientFunds),
            _ => PaymentResult.Failure(ErrorCodes.PaymentFailed)
        };

        if (result.IsSuccess)
            _logger.LogInformation("Payment {Reference} succeeded: {Brand} ****{Last4}, {Amount} OMR",
                result.Reference, brand, last4, amount);
        else
            _logger.LogWarning("Payment refused with {Code}: {Brand} ****{Last4}",
                result.FailureCode, brand, last4);

        return result;
    }

    public async Task<bool> RefundAsync(string paymentReference, decimal amount, CancellationToken cancellationToken)
    {
        await Task.Delay(SimulatedLatency, cancellationToken);

        if (amount <= 0 || !IsKnownReference(paymentReference))
        {
            _logger.LogWarning("Refund failed for {Reference}: unknown reference or invalid amount", paymentReference);
            return false;
        }

        _logger.LogInformation("Refund of {Amount} OMR issued for payment {Reference}", amount, paymentReference);
        return true;
    }

    private static bool TryParseToken(string? token, out string outcome, out string brand, out string last4)
    {
        outcome = brand = last4 = string.Empty;

        var parts = token?.Trim().ToLowerInvariant().Split('_');
        if (parts is null || parts.Length != TokenParts || parts[0] != TokenPrefix)
            return false;

        var cardBrand = parts[2] switch
        {
            "visa" => "Visa",
            "mastercard" => "Mastercard",
            _ => null
        };

        if (cardBrand is null || parts[3].Length != 4 || !parts[3].All(char.IsAsciiDigit))
            return false;

        outcome = parts[1];
        brand = cardBrand;
        last4 = parts[3];
        return true;
    }

    private static bool IsKnownReference(string? reference)
        => !string.IsNullOrWhiteSpace(reference)
           && reference.StartsWith(ReferencePrefix, StringComparison.Ordinal);

    private static string NewReference()
        => ReferencePrefix + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
}