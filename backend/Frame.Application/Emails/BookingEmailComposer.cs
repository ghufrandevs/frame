using System.Globalization;
using System.Net;
using Frame.Application.Common.Abstractions;
using Frame.Application.Studios;
using Frame.Domain.Entities;
using Frame.Domain.Enums;

namespace Frame.Application.Emails;

/// <summary>Subject and HTML body of one email, ready to send.</summary>
internal sealed record EmailContent(string Subject, string HtmlBody);

/// <summary>
/// Builds the booking emails (confirmation with invoice, cancellation with refund)
/// in the customer's language, saved on the message at booking time.
/// Every value typed by a user is HTML-encoded, so a name can never inject links or markup.
/// </summary>
internal sealed class BookingEmailComposer
{
    private const string ArabicCode = "ar";

    private readonly IClock _clock;

    public BookingEmailComposer(IClock clock)
    {
        _clock = clock;
    }

    /// <summary>Requires message.Booking with its Studio and User loaded.</summary>
    public EmailContent Compose(EmailMessage message)
    {
        var booking = message.Booking;
        var isArabic = message.Language == ArabicCode;
        var t = isArabic ? Texts.Arabic : Texts.English;
        var isCancelled = message.Type == EmailType.BookingCancelled;

        var subject = $"{(isCancelled ? t.CancelledSubject : t.ConfirmedSubject)} {booking.BookingNumber} · Frame";

        var rows = new List<(string Label, string Value)>
        {
            (t.BookingNumber, booking.BookingNumber),
            (t.Studio, booking.Studio.LocalizedName(isArabic)),
            (t.Date, booking.BookingDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)),
            (t.Time, $"{booking.StartHour:00}:00–{booking.EndHour:00}:00"),
            (t.InvoiceNumber, booking.InvoiceNumber),
            (t.IssuedAt, FormatTime(booking.CreatedAt)),
            (t.Hours, booking.Hours.ToString(CultureInfo.InvariantCulture)),
            (t.HourlyRate, Money(booking.HourlyRate, t))
        };

        // Only shown when the customer chose a photographer.
        if (booking.PhotographerFee > 0)
            rows.Add((t.Photographer, Money(booking.PhotographerFee, t)));

        rows.Add((t.Subtotal, Money(booking.Subtotal, t)));
        rows.Add((t.Vat, Money(booking.VatAmount, t)));
        rows.Add((t.Total, Money(booking.TotalAmount, t)));
        rows.Add((t.Payment, $"{booking.CardBrand} •••• {booking.CardLast4} · {booking.PaymentReference}"));

        if (isCancelled)
        {
            rows.Add((t.Refund, Money(booking.RefundAmount ?? booking.TotalAmount, t)));
            if (booking.CancelledAt is { } cancelledAt)
                rows.Add((t.CancelledAt, FormatTime(cancelledAt)));
        }

        var body = BuildHtml(
            isArabic,
            string.Format(CultureInfo.InvariantCulture, t.Greeting, booking.User.FullName),
            isCancelled ? t.CancelledIntro : t.ConfirmedIntro,
            rows,
            t.Footer);

        return new EmailContent(subject, body);
    }

    private string FormatTime(DateTime utc)
        => _clock.ToMuscat(utc).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

    private static string Money(decimal amount, Texts t)
        => $"{amount.ToString("0.000", CultureInfo.InvariantCulture)} {t.Currency}";

    private static string BuildHtml(
        bool isArabic,
        string greeting,
        string intro,
        IEnumerable<(string Label, string Value)> rows,
        string footer)
    {
        var dir = isArabic ? "rtl" : "ltr";
        var align = isArabic ? "right" : "left";

        var tableRows = string.Concat(rows.Select(r =>
            $"<tr><td style=\"padding:8px 12px;color:#666;border-bottom:1px solid #eee\">{Encode(r.Label)}</td>" +
            $"<td style=\"padding:8px 12px;font-weight:600;border-bottom:1px solid #eee\">{Encode(r.Value)}</td></tr>"));

        return $"""
            <!DOCTYPE html>
            <html lang="{(isArabic ? "ar" : "en")}" dir="{dir}">
            <body style="margin:0;padding:24px;background:#f6f6f4;font-family:Tahoma,Arial,sans-serif;text-align:{align}">
              <div style="max-width:560px;margin:0 auto;background:#fff;border-radius:12px;padding:28px">
                <h1 style="margin:0 0 20px;font-size:22px">Frame</h1>
                <p style="margin:0 0 8px">{Encode(greeting)}</p>
                <p style="margin:0 0 20px;color:#333">{Encode(intro)}</p>
                <table style="width:100%;border-collapse:collapse;font-size:14px">{tableRows}</table>
                <p style="margin:24px 0 0;color:#888;font-size:12px">{Encode(footer)}</p>
              </div>
            </body>
            </html>
            """;
    }

    private static string Encode(string value) => WebUtility.HtmlEncode(value);

    /// <summary>All wording of one language. The HTML layout is shared. Greeting has {0} for the name.</summary>
    private sealed record Texts(
        string ConfirmedSubject, string CancelledSubject,
        string Greeting, string ConfirmedIntro, string CancelledIntro,
        string BookingNumber, string Studio, string Date, string Time,
        string InvoiceNumber, string IssuedAt, string Hours, string HourlyRate,
        string Photographer, string Subtotal, string Vat, string Total, string Payment,
        string Refund, string CancelledAt, string Currency, string Footer)
    {
        public static readonly Texts Arabic = new(
            ConfirmedSubject: "تأكيد حجزك",
            CancelledSubject: "إلغاء حجزك",
            Greeting: "مرحباً {0}،",
            ConfirmedIntro: "تم تأكيد حجزك ودفع المبلغ بنجاح. هذه تفاصيل الحجز والفاتورة.",
            CancelledIntro: "نعتذر منك، تم إلغاء حجزك من إدارة Frame، وتم استرداد المبلغ كاملاً إلى بطاقتك.",
            BookingNumber: "رقم الحجز",
            Studio: "الاستوديو",
            Date: "التاريخ",
            Time: "الوقت",
            InvoiceNumber: "رقم الفاتورة",
            IssuedAt: "تاريخ الإصدار",
            Hours: "عدد الساعات",
            HourlyRate: "سعر الساعة",
            Photographer: "مصوّر",
            Subtotal: "المجموع",
            Vat: "ضريبة القيمة المضافة 5٪",
            Total: "الإجمالي",
            Payment: "طريقة الدفع",
            Refund: "المبلغ المسترد",
            CancelledAt: "تاريخ الإلغاء",
            Currency: "ر.ع.",
            Footer: "Frame · استوديوهات المحتوى بالساعة · مسقط");

        public static readonly Texts English = new(
            ConfirmedSubject: "Your booking is confirmed",
            CancelledSubject: "Your booking was cancelled",
            Greeting: "Hello {0},",
            ConfirmedIntro: "Your booking is confirmed and your payment was successful. Here are your booking and invoice details.",
            CancelledIntro: "We are sorry: Frame cancelled your booking and refunded the full amount to your card.",
            BookingNumber: "Booking number",
            Studio: "Studio",
            Date: "Date",
            Time: "Time",
            InvoiceNumber: "Invoice number",
            IssuedAt: "Issued at",
            Hours: "Hours",
            HourlyRate: "Hourly rate",
            Photographer: "Photographer",
            Subtotal: "Subtotal",
            Vat: "VAT 5%",
            Total: "Total",
            Payment: "Payment",
            Refund: "Refunded",
            CancelledAt: "Cancelled at",
            Currency: "OMR",
            Footer: "Frame · Content studios by the hour · Muscat");
    }
}