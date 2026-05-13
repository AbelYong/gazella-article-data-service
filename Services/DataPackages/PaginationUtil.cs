using ArticleService.Services.Exceptions;

namespace ArticleService.Services.DataPackages;

public static class PaginationUtil
{
    private const int MinPageIndex = 0;
    private const int MinPageSize = 10;
    private const int MaxPageSize = 50;
    private const int MaxPageIndex = (int.MaxValue / MinPageSize) - 1;
    private const int MinOffset = 0;
    private const int MaxOffset = (int.MaxValue / MaxPageSize) - MaxPageSize;

    /// <summary>
    /// Validates if the provided index is within the accepted range
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="GazellaValidationException">Thrown only if the page index is outside the accepted bounds</exception>
    public static void ValidatePageIndex(int index)
    {
        switch (index)
        {
            case < MinPageIndex:
                throw new GazellaValidationException($"Page index is too low, received {index}, lowest is {MinPageIndex}");
            case > MaxPageIndex:
                throw new GazellaValidationException(
                    $"Page index is too high, received {index}, highest is {MaxPageIndex}");
        }
    }

    /// <summary>
    /// Validates if the provided page size is within the accepted range
    /// </summary>
    /// <param name="size"></param>
    /// <exception cref="GazellaValidationException">Thrown only if the page size is outside the accepted bounds</exception>
    public static void ValidatePageSize(int size)
    {
        switch (size)
        {
            case < MinPageSize:
                throw new GazellaValidationException($"Page size is too low, received {size}, lowest is {MinPageSize}");
            case > MaxPageSize:
                throw new GazellaValidationException($"Page size is too high, received {size}, highest is {MaxPageSize}");
        }
    }

    /// <summary>
    /// Validates if the calculated skip offset is within the accepted range given a pageIndex and pageSize
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <exception cref="GazellaValidationException">Thrown if the calculated skip offset is outside the accepted bounds</exception>
    public static void ValidatePageOffset(int pageIndex, int pageSize)
    {
        long offset = pageIndex * pageSize;

        switch (offset)
        {
            case < MinOffset:
                throw new GazellaValidationException($"Skip offset is too low, calculated {offset}, lowest is {MinOffset}");
            case > MaxOffset:
                throw new GazellaValidationException($"Skip offset  is too high, calculated {offset}, highest is {MaxOffset}");
        }
    }

    /// <summary>
    /// Returns the calculated offset. Does not guarantee the returned offset will be valid 
    /// </summary>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static int GetOffset(int pageIndex, int pageSize)
    {
        return pageIndex * pageSize;
    }
}