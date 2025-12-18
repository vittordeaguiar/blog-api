using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Interfaces;

namespace BlogAPI.Infrastructure.Services;

public partial class SlugGenerator : ISlugGenerator
{
    private readonly IPostRepository _postRepository;
    private const int MaxSlugLength = 200;
    private const int MaxAttempts = 10000;

    [GeneratedRegex(@"^[a-z0-9]+(?:-[a-z0-9]+)*$")]
    private static partial Regex SlugValidationRegex();

    [GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial Regex InvalidCharsRegex();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex MultipleHyphensRegex();

    public SlugGenerator(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new DomainException("Text cannot be empty");
        }

        var normalized = RemoveDiacritics(text);
        var lowercased = normalized.ToLowerInvariant();
        var withHyphens = lowercased.Replace(' ', '-');
        var cleaned = InvalidCharsRegex().Replace(withHyphens, "-");
        var singleHyphens = MultipleHyphensRegex().Replace(cleaned, "-");
        var trimmed = singleHyphens.Trim('-');

        if (trimmed.Length > MaxSlugLength)
        {
            trimmed = trimmed[..MaxSlugLength];
            var lastHyphenIndex = trimmed.LastIndexOf('-');
            if (lastHyphenIndex > 0)
            {
                trimmed = trimmed[..lastHyphenIndex];
            }
        }

        if (string.IsNullOrWhiteSpace(trimmed) || !SlugValidationRegex().IsMatch(trimmed))
        {
            throw new DomainException("Text must contain at least one alphanumeric character");
        }

        return trimmed;
    }

    public async Task<string> GenerateUniqueSlugAsync(string baseSlug, Guid? excludePostId = null)
    {
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            throw new DomainException("Base slug cannot be empty");
        }

        var candidateSlug = baseSlug;
        var attempt = 1;

        while (attempt <= MaxAttempts)
        {
            if (!await SlugExistsAsync(candidateSlug, excludePostId))
            {
                return candidateSlug;
            }

            attempt++;
            candidateSlug = $"{baseSlug}-{attempt}";
        }

        throw new DomainException($"Failed to generate unique slug after {MaxAttempts} attempts");
    }

    private async Task<bool> SlugExistsAsync(string slug, Guid? excludePostId)
    {
        var existingPost = await _postRepository.GetBySlugAsync(slug);

        if (existingPost == null)
        {
            return false;
        }

        if (excludePostId.HasValue && existingPost.Id == excludePostId.Value)
        {
            return false;
        }

        return true;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
