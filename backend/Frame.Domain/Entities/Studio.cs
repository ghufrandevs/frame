using Frame.Domain.Common;

namespace Frame.Domain.Entities;

/// <summary>
/// A bookable content studio (e.g. White Studio, Podcast Room).
/// Names and descriptions are stored in Arabic and English;
/// the API returns the one matching the request language.
/// </summary>
public sealed class Studio : BaseEntity
{
    public string NameAr { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string DescriptionAr { get; private set; } = null!;
    public string DescriptionEn { get; private set; } = null!;
    public string ImageUrl { get; private set; } = null!;

    /// <summary>Price for one hour in OMR (3 decimal places). Changing it affects new bookings only.</summary>
    public decimal PricePerHour { get; private set; }

    /// <summary>First bookable hour, 24h clock in Muscat time (e.g. 9 = 09:00).</summary>
    public int OpenHour { get; private set; }

    /// <summary>Hour the studio closes (e.g. 22 = 22:00). The last booking must end by this hour.</summary>
    public int CloseHour { get; private set; }

    /// <summary>False = paused for maintenance: hidden from customers, existing bookings stay.</summary>
    public bool IsActive { get; private set; }

    private Studio() { }

    public static Studio Create(
        string nameAr, string nameEn,
        string descriptionAr, string descriptionEn,
        string imageUrl, decimal pricePerHour,
        int openHour, int closeHour)
    {
        var studio = new Studio { IsActive = true };
        studio.UpdateDetails(nameAr, nameEn, descriptionAr, descriptionEn, imageUrl);
        studio.ChangePrice(pricePerHour);
        studio.ChangeOpeningHours(openHour, closeHour);
        return studio;
    }

    public void UpdateDetails(
        string nameAr, string nameEn,
        string descriptionAr, string descriptionEn,
        string imageUrl)
    {
        NameAr = Guard.NotEmpty(nameAr, "STUDIO_NAME_AR_REQUIRED");
        NameEn = Guard.NotEmpty(nameEn, "STUDIO_NAME_EN_REQUIRED");
        DescriptionAr = Guard.NotEmpty(descriptionAr, "STUDIO_DESCRIPTION_AR_REQUIRED");
        DescriptionEn = Guard.NotEmpty(descriptionEn, "STUDIO_DESCRIPTION_EN_REQUIRED");
        ImageUrl = Guard.NotEmpty(imageUrl, "STUDIO_IMAGE_REQUIRED");
    }

    public void ChangePrice(decimal pricePerHour)
    {
        PricePerHour = Guard.Positive(pricePerHour, "STUDIO_PRICE_INVALID");
    }

    public void ChangeOpeningHours(int openHour, int closeHour)
    {
        if (openHour < 0 || closeHour > 24 || openHour >= closeHour)
            throw new DomainException("STUDIO_HOURS_INVALID");

        OpenHour = openHour;
        CloseHour = closeHour;
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;

    /// <summary>
    /// True if a booking from startHour to endHour fits inside opening hours.
    /// Used by availability and booking checks, so the rule lives in one place.
    /// </summary>
    public bool IsWithinOpeningHours(int startHour, int endHour)
        => startHour >= OpenHour && endHour <= CloseHour && startHour < endHour;
}
