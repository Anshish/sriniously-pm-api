namespace Core.DTOs
{
    public record PagedResponse<T>(
        IReadOnlyList<T> Data,
        int Total,
        int Page,
        int TotalPages
    );

    public record PaginationParams(
        int Page=1,
        int Limit=10,
        string? SortBy="createdAt",
        string? SortOrder="desc",
        string? Status=null,
        string? Name=null
    );
}