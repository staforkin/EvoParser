internal class EvoProduct
{
    public EvoProduct()
    {
    }

    public EvoProduct(int id, string name, Uri uri, Uri imageUri, double regularPrice, double? outletPrice, bool inStoreOnly, double? rating, int? reviewsCount, bool clearance)
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
        Clearance = clearance;
    }

    public int Id { get; private set; }

    public string Name { get; private set; }

    public Uri? Uri { get; private set; }

    public Uri? ImageUri { get; private set; }

    public double RegularPrice { get; private set; }

    public double? OutletPrice { get; private set; }

    public double? DiscountPercent
    {
        get
        {
            double? saving = null;
            if (OutletPrice.HasValue)
            {
                saving = (RegularPrice - OutletPrice.Value) / RegularPrice;
            }
            return saving;
        }
    }

    public bool InStoreOnly { get; private set; }

    public Double? Rating { get; set; }

    public int? ReviewsCount { get; set; }

    public bool Clearance { get; private set; }
}