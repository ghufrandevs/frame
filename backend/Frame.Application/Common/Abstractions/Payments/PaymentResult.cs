namespace Frame.Application.Common.Abstractions.Payments;

/// <summary>
/// Outcome of a charge. A declined card is a normal result, not an exception:
/// the booking service reads FailureCode and answers 402 to the customer.
/// Never holds the full card number or CVV, only the brand and last 4 digits.
/// </summary>
public sealed record PaymentResult
{
    public bool IsSuccess { get; }
    public string? Reference { get; }
    public string? CardBrand { get; }
    public string? CardLast4 { get; }
    public string? FailureCode { get; }

    private PaymentResult(bool isSuccess, string? reference, string? cardBrand, string? cardLast4, string? failureCode)
    {
        IsSuccess = isSuccess;
        Reference = reference;
        CardBrand = cardBrand;
        CardLast4 = cardLast4;
        FailureCode = failureCode;
    }

    public static PaymentResult Success(string reference, string cardBrand, string cardLast4)
        => new(true, reference, cardBrand, cardLast4, null);

    public static PaymentResult Failure(string failureCode)
        => new(false, null, null, null, failureCode);
}