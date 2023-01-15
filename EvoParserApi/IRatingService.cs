

internal interface IRatingService
{
    Task<ReviewServiceResponse> GetRatingsAsync(int[] ids);
}
