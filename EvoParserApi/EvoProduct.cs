
using System.Globalization;

internal class EvoProduct
{
    public EvoProduct(int id, string name, Uri uri, Uri imageUri, string regularPrice, string? outletPrice, bool inStoreOnly, double? rating, int? reviewsCount)
    {
        Id = id;
        Name = name;
        Uri = uri;
        ImageUri = imageUri;
        RegularPrice = regularPrice;
        OutletPrice = outletPrice;
        InStoreOnly = inStoreOnly;
        Rating = rating;
        ReviewsCount = reviewsCount;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public Uri? Uri { get; private set; }

    public Uri? ImageUri { get; private set; }

    public string RegularPrice { get; private set; }

    public string? OutletPrice { get; private set; }

    public double? DiscountPercent
    {
        get
        {
            double price = default;
            price = double.Parse(RegularPrice.Substring(1), CultureInfo.InvariantCulture);

            double? outlet = OutletPrice == null ? null : double.Parse(OutletPrice.Substring(1), CultureInfo.InvariantCulture);
            double? saving = null;
            if (outlet.HasValue)
            {
                saving = (price - outlet) * 100 / price;
            }
            return saving;
        }
    }

    public bool InStoreOnly { get; private set; }

    public Double? Rating { get; private set; }

    public int? ReviewsCount { get; private set; }
}