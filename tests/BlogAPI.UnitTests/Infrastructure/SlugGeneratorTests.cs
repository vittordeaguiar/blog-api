using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Interfaces;
using BlogAPI.Infrastructure.Services;
using FluentAssertions;
using Moq;

namespace BlogAPI.UnitTests.Infrastructure;

public class SlugGeneratorTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly SlugGenerator _slugGenerator;

    public SlugGeneratorTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _slugGenerator = new SlugGenerator(_postRepositoryMock.Object);
    }

    private static Post CreateTestPost(string slug)
    {
        return new Post("Test Title", "Test content with enough characters", slug, Guid.NewGuid());
    }

    #region GenerateSlug Tests

    [Fact]
    public void GenerateSlug_WithValidText_ReturnsSlug()
    {
        var result = _slugGenerator.GenerateSlug("Hello World");

        result.Should().Be("hello-world");
    }

    [Fact]
    public void GenerateSlug_WithAccents_RemovesAccents()
    {
        var result = _slugGenerator.GenerateSlug("Café com Açúcar");

        result.Should().Be("cafe-com-acucar");
    }

    [Fact]
    public void GenerateSlug_WithSpecialCharacters_ReplacesWithHyphens()
    {
        var result = _slugGenerator.GenerateSlug("Meu Post Incrível!");

        result.Should().Be("meu-post-incrivel");
    }

    [Fact]
    public void GenerateSlug_WithMultipleSpaces_UsesSingleHyphen()
    {
        var result = _slugGenerator.GenerateSlug("Multiple   Spaces");

        result.Should().Be("multiple-spaces");
    }

    [Fact]
    public void GenerateSlug_WithUppercase_ConvertsToLowercase()
    {
        var result = _slugGenerator.GenerateSlug("HELLO WORLD");

        result.Should().Be("hello-world");
    }

    [Fact]
    public void GenerateSlug_WithLeadingTrailingSpaces_TrimsHyphens()
    {
        var result = _slugGenerator.GenerateSlug("  Hello World  ");

        result.Should().Be("hello-world");
    }

    [Fact]
    public void GenerateSlug_WithNumbers_PreservesNumbers()
    {
        var result = _slugGenerator.GenerateSlug("Hello 123 World");

        result.Should().Be("hello-123-world");
    }

    [Fact]
    public void GenerateSlug_WithPortugueseAccents_RemovesAllAccents()
    {
        var result = _slugGenerator.GenerateSlug("Ação Coração Até Pêssego");

        result.Should().Be("acao-coracao-ate-pessego");
    }

    [Fact]
    public void GenerateSlug_WithEmptyString_ThrowsException()
    {
        var act = () => _slugGenerator.GenerateSlug("");

        act.Should().Throw<DomainException>().WithMessage("Text cannot be empty");
    }

    [Fact]
    public void GenerateSlug_WithWhitespaceOnly_ThrowsException()
    {
        var act = () => _slugGenerator.GenerateSlug("   ");

        act.Should().Throw<DomainException>().WithMessage("Text cannot be empty");
    }

    [Fact]
    public void GenerateSlug_WithOnlySpecialCharacters_ThrowsException()
    {
        var act = () => _slugGenerator.GenerateSlug("!@#$%^&*()");

        act.Should().Throw<DomainException>().WithMessage("Text must contain at least one alphanumeric character");
    }

    [Fact]
    public void GenerateSlug_WithVeryLongText_TruncatesTo200Characters()
    {
        var longText = new string('a', 250);
        var result = _slugGenerator.GenerateSlug(longText);

        result.Should().HaveLength(200);
    }

    [Fact]
    public void GenerateSlug_WithLongTextAndHyphens_TruncatesAtLastHyphen()
    {
        var longText = new string('a', 195) + " hello world extra";
        var result = _slugGenerator.GenerateSlug(longText);

        result.Should().HaveLength(195);
        result.Should().NotEndWith("-");
    }

    #endregion

    #region GenerateUniqueSlugAsync Tests

    [Fact]
    public async Task GenerateUniqueSlugAsync_WithAvailableSlug_ReturnsSlug()
    {
        var baseSlug = "hello-world";
        _postRepositoryMock.Setup(r => r.GetBySlugAsync(baseSlug)).ReturnsAsync((Post?)null);

        var result = await _slugGenerator.GenerateUniqueSlugAsync(baseSlug);

        result.Should().Be("hello-world");
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_WithExistingSlug_AppendsSuffix()
    {
        var baseSlug = "hello-world";
        var existingPost = CreateTestPost(baseSlug);

        _postRepositoryMock.Setup(r => r.GetBySlugAsync(baseSlug)).ReturnsAsync(existingPost);
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("hello-world-2")).ReturnsAsync((Post?)null);

        var result = await _slugGenerator.GenerateUniqueSlugAsync(baseSlug);

        result.Should().Be("hello-world-2");
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_WithMultipleExistingSlugs_FindsNextAvailable()
    {
        var baseSlug = "hello-world";
        var post1 = CreateTestPost(baseSlug);
        var post2 = CreateTestPost("hello-world-2");
        var post3 = CreateTestPost("hello-world-3");

        _postRepositoryMock.Setup(r => r.GetBySlugAsync(baseSlug)).ReturnsAsync(post1);
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("hello-world-2")).ReturnsAsync(post2);
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("hello-world-3")).ReturnsAsync(post3);
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("hello-world-4")).ReturnsAsync((Post?)null);

        var result = await _slugGenerator.GenerateUniqueSlugAsync(baseSlug);

        result.Should().Be("hello-world-4");
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_WithExcludePostId_AllowsSameSlug()
    {
        var postId = Guid.NewGuid();
        var baseSlug = "hello-world";
        var existingPost = CreateTestPost(baseSlug);
        typeof(Post).GetProperty("Id")!.SetValue(existingPost, postId);

        _postRepositoryMock.Setup(r => r.GetBySlugAsync(baseSlug)).ReturnsAsync(existingPost);

        var result = await _slugGenerator.GenerateUniqueSlugAsync(baseSlug, postId);

        result.Should().Be("hello-world");
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_WithExcludePostIdButOtherExists_AppendsSuffix()
    {
        var postId1 = Guid.NewGuid();
        var postId2 = Guid.NewGuid();
        var baseSlug = "hello-world";
        var post1 = CreateTestPost(baseSlug);
        typeof(Post).GetProperty("Id")!.SetValue(post1, postId1);

        _postRepositoryMock.Setup(r => r.GetBySlugAsync(baseSlug)).ReturnsAsync(post1);
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("hello-world-2")).ReturnsAsync((Post?)null);

        var result = await _slugGenerator.GenerateUniqueSlugAsync(baseSlug, postId2);

        result.Should().Be("hello-world-2");
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_WithEmptyBaseSlug_ThrowsException()
    {
        var act = async () => await _slugGenerator.GenerateUniqueSlugAsync("");

        await act.Should().ThrowAsync<DomainException>().WithMessage("Base slug cannot be empty");
    }

    [Fact]
    public async Task GenerateUniqueSlugAsync_WithWhitespaceBaseSlug_ThrowsException()
    {
        var act = async () => await _slugGenerator.GenerateUniqueSlugAsync("   ");

        await act.Should().ThrowAsync<DomainException>().WithMessage("Base slug cannot be empty");
    }

    #endregion
}
