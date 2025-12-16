namespace BlogAPI.API.Configuration;

public static class CacheKeys
{
    private const string PostsPrefix = "posts";
    private const string CategoriesPrefix = "categories";

    public static string PostsPage(int pageNumber, int pageSize, string? category, string? search) =>
        $"{PostsPrefix}:page:{pageNumber}:{pageSize}:{category ?? "all"}:{search ?? "none"}";

    public static string PostById(Guid id) =>
        $"{PostsPrefix}:id:{id}";

    public static string PostBySlug(string slug) =>
        $"{PostsPrefix}:slug:{slug}";

    public static string AllPosts() =>
        $"{PostsPrefix}:*";

    public static string Categories() =>
        $"{CategoriesPrefix}:all";

    public static string CategoryById(Guid id) =>
        $"{CategoriesPrefix}:id:{id}";

    public static string AllCategories() =>
        $"{CategoriesPrefix}:*";
}
