using AutoMapper;
using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using BlogAPI.Application.Mappings;
using BlogAPI.Application.Services;
using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace BlogAPI.UnitTests.Application;

public class PostServiceSlugTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<ISlugGenerator> _slugGeneratorMock;
    private readonly PostService _postService;
    private readonly User _testAuthor;
    private readonly Guid _authorId;

    public PostServiceSlugTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _cacheServiceMock = new Mock<ICacheService>();
        _slugGeneratorMock = new Mock<ISlugGenerator>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<PostMappingProfile>());
        var mapper = config.CreateMapper();

        _postService = new PostService(
            _postRepositoryMock.Object,
            _userRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _cacheServiceMock.Object,
            _slugGeneratorMock.Object,
            mapper
        );

        _authorId = Guid.NewGuid();
        _testAuthor = new User("John Doe", "john@example.com", "hash", "Author");
        _userRepositoryMock.Setup(r => r.GetByIdAsync(_authorId)).ReturnsAsync(_testAuthor);
    }

    #region CreatePostAsync - Automatic Slug Generation

    [Fact]
    public async Task CreatePostAsync_WithoutSlug_GeneratesSlugAutomatically()
    {
        var dto = new CreatePostDto(
            Title: "My Amazing Blog Post",
            Content: "This is great content with enough characters"
        );

        _slugGeneratorMock
            .Setup(s => s.GenerateSlug("My Amazing Blog Post"))
            .Returns("my-amazing-blog-post");

        _slugGeneratorMock
            .Setup(s => s.GenerateUniqueSlugAsync("my-amazing-blog-post", null))
            .ReturnsAsync("my-amazing-blog-post");

        _postRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Post>()))
            .ReturnsAsync((Post p) => p);

        var result = await _postService.CreatePostAsync(dto, _authorId);

        result.Slug.Should().Be("my-amazing-blog-post");
        _slugGeneratorMock.Verify(s => s.GenerateSlug("My Amazing Blog Post"), Times.Once);
        _slugGeneratorMock.Verify(s => s.GenerateUniqueSlugAsync("my-amazing-blog-post", null), Times.Once);
    }

    [Fact]
    public async Task CreatePostAsync_WithoutSlug_WhenSlugExists_GeneratesUniqueSuffix()
    {
        var dto = new CreatePostDto(
            Title: "Hello World",
            Content: "Content with enough characters here"
        );

        _slugGeneratorMock
            .Setup(s => s.GenerateSlug("Hello World"))
            .Returns("hello-world");

        _slugGeneratorMock
            .Setup(s => s.GenerateUniqueSlugAsync("hello-world", null))
            .ReturnsAsync("hello-world-2");

        _postRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Post>()))
            .ReturnsAsync((Post p) => p);

        var result = await _postService.CreatePostAsync(dto, _authorId);

        result.Slug.Should().Be("hello-world-2");
    }

    [Fact]
    public async Task CreatePostAsync_WithCustomSlug_UsesProvidedSlug()
    {
        var dto = new CreatePostDto(
            Title: "Some Title",
            Content: "Some content with enough characters",
            Slug: "custom-slug"
        );

        _postRepositoryMock
            .Setup(r => r.GetBySlugAsync("custom-slug"))
            .ReturnsAsync((Post?)null);

        _postRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Post>()))
            .ReturnsAsync((Post p) => p);

        var result = await _postService.CreatePostAsync(dto, _authorId);

        result.Slug.Should().Be("custom-slug");
        _slugGeneratorMock.Verify(s => s.GenerateSlug(It.IsAny<string>()), Times.Never);
        _slugGeneratorMock.Verify(s => s.GenerateUniqueSlugAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
    }

    [Fact]
    public async Task CreatePostAsync_WithCustomSlugThatExists_ThrowsException()
    {
        var dto = new CreatePostDto(
            Title: "New Post",
            Content: "Content with enough characters",
            Slug: "existing-slug"
        );

        var existingPost = new Post("Old Post", "Old content here", "existing-slug", Guid.NewGuid());
        _postRepositoryMock
            .Setup(r => r.GetBySlugAsync("existing-slug"))
            .ReturnsAsync(existingPost);

        var act = async () => await _postService.CreatePostAsync(dto, _authorId);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Slug 'existing-slug' is already in use");
    }

    #endregion

    #region UpdatePostAsync - Automatic Slug Generation

    [Fact]
    public async Task UpdatePostAsync_WithoutSlug_RegeneratesSlugFromTitle()
    {
        var postId = Guid.NewGuid();
        var existingPost = new Post("Old Title", "Old content with enough characters", "old-slug", _authorId);
        typeof(Post).GetProperty("Id")!.SetValue(existingPost, postId);

        var dto = new UpdatePostDto(
            Title: "New Title",
            Content: "New content with enough characters"
        );

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(existingPost);

        _slugGeneratorMock
            .Setup(s => s.GenerateSlug("New Title"))
            .Returns("new-title");

        _slugGeneratorMock
            .Setup(s => s.GenerateUniqueSlugAsync("new-title", postId))
            .ReturnsAsync("new-title");

        var result = await _postService.UpdatePostAsync(postId, dto);

        result.Slug.Should().Be("new-title");
        _slugGeneratorMock.Verify(s => s.GenerateSlug("New Title"), Times.Once);
        _slugGeneratorMock.Verify(s => s.GenerateUniqueSlugAsync("new-title", postId), Times.Once);
    }

    [Fact]
    public async Task UpdatePostAsync_WithoutSlug_ExcludesCurrentPostFromUniquenessCheck()
    {
        var postId = Guid.NewGuid();
        var existingPost = new Post("Same Title", "Content with enough characters", "same-title", _authorId);
        typeof(Post).GetProperty("Id")!.SetValue(existingPost, postId);

        var dto = new UpdatePostDto(
            Title: "Same Title",
            Content: "Updated content with enough characters"
        );

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(existingPost);

        _slugGeneratorMock
            .Setup(s => s.GenerateSlug("Same Title"))
            .Returns("same-title");

        _slugGeneratorMock
            .Setup(s => s.GenerateUniqueSlugAsync("same-title", postId))
            .ReturnsAsync("same-title");

        var result = await _postService.UpdatePostAsync(postId, dto);

        result.Slug.Should().Be("same-title");
        _slugGeneratorMock.Verify(s => s.GenerateUniqueSlugAsync("same-title", postId), Times.Once);
    }

    [Fact]
    public async Task UpdatePostAsync_WithCustomSlug_UsesProvidedSlug()
    {
        var postId = Guid.NewGuid();
        var existingPost = new Post("Old Title", "Old content with enough characters", "old-slug", _authorId);
        typeof(Post).GetProperty("Id")!.SetValue(existingPost, postId);

        var dto = new UpdatePostDto(
            Title: "New Title",
            Content: "New content with enough characters",
            Slug: "custom-new-slug"
        );

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(existingPost);
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("custom-new-slug")).ReturnsAsync((Post?)null);

        var result = await _postService.UpdatePostAsync(postId, dto);

        result.Slug.Should().Be("custom-new-slug");
        _slugGeneratorMock.Verify(s => s.GenerateSlug(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task UpdatePostAsync_WithCustomSlugThatExistsOnOtherPost_ThrowsException()
    {
        var postId1 = Guid.NewGuid();
        var postId2 = Guid.NewGuid();

        var post1 = new Post("Post 1", "Content with enough characters", "post-1", _authorId);
        typeof(Post).GetProperty("Id")!.SetValue(post1, postId1);

        var post2 = new Post("Post 2", "Content with enough characters", "post-2", _authorId);
        typeof(Post).GetProperty("Id")!.SetValue(post2, postId2);

        var dto = new UpdatePostDto(
            Title: "Updated Title",
            Content: "Updated content with enough characters",
            Slug: "post-2"
        );

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId1)).ReturnsAsync(post1);
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("post-2")).ReturnsAsync(post2);

        var act = async () => await _postService.UpdatePostAsync(postId1, dto);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Slug 'post-2' is already in use");
    }

    [Fact]
    public async Task UpdatePostAsync_WithSameCustomSlugAsCurrentPost_AllowsUpdate()
    {
        var postId = Guid.NewGuid();
        var existingPost = new Post("Title", "Content with enough characters", "my-slug", _authorId);
        typeof(Post).GetProperty("Id")!.SetValue(existingPost, postId);

        var dto = new UpdatePostDto(
            Title: "Updated Title",
            Content: "Updated content with enough characters",
            Slug: "my-slug"
        );

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(existingPost);
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("my-slug")).ReturnsAsync(existingPost);

        var result = await _postService.UpdatePostAsync(postId, dto);

        result.Slug.Should().Be("my-slug");
        _postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Once);
    }

    #endregion
}
