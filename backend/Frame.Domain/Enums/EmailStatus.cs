namespace Frame.Domain.Enums;

/// <summary>
/// Delivery state of a queued email. The background sender
/// picks up Pending messages and moves them to Sent or Failed.
/// </summary>
public enum EmailStatus
{
    /// <summary>Saved with the booking, waiting for the background sender.</summary>
    Pending = 1,

    /// <summary>Delivered to the mail server successfully.</summary>
    Sent = 2,

    /// <summary>Gave up after the maximum number of attempts. The error is kept for review.</summary>
    Failed = 3
}
