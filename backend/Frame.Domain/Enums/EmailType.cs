namespace Frame.Domain.Enums;

/// <summary>
/// Which email the customer receives. Each type has its own
/// bilingual template (ar/en) in the Infrastructure layer.
/// </summary>
public enum EmailType
{
    /// <summary>Sent right after a successful payment: booking details + invoice.</summary>
    BookingConfirmed = 1,

    /// <summary>Sent after an admin cancels a booking: cancellation + refunded amount.</summary>
    BookingCancelled = 2
}
