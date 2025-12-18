namespace BlogAPI.Domain.Interfaces;

public interface ISlugGenerator
{
    string GenerateSlug(string text);

    Task<string> GenerateUniqueSlugAsync(string baseSlug, Guid? excludePostId = null);
}
