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

public class PostServiceTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly PostService _postService;

    public PostServiceTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<PostMappingProfile>());
        var mapper = config.CreateMapper();

        _postService = new PostService(
            _postRepositoryMock.Object,
            _userRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            mapper
        );
    }

    private static Post CreateTestPost(string title, string content, string slug, Guid? authorId = null)
    {
        var author = new User("Test Author", "test@example.com", "hash", "Author");
        var post = new Post(title, content, slug, authorId ?? author.Id);
        typeof(Post).GetProperty("Author")!.SetValue(post, author);
        return post;
    }

    #region CreatePostAsync Tests

    [Fact]
    public async Task CreatePostAsync_WithValidData_CreatesPost()
    {
        var dto = new CreatePostDto("Test Post Title", "Valid content with more than 10 characters", "test-post-title");
        var authorId = Guid.NewGuid();
        var author = new User("John Doe", "john@example.com", "hash", "Author");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(author);

        _postRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Post>()))
            .Callback<Post>(p => _ = p)
            .ReturnsAsync((Post p) => p);

        var result = await _postService.CreatePostAsync(dto, authorId);

        result.Should().NotBeNull();
        result.Title.Should().Be(dto.Title);
        result.Content.Should().Be(dto.Content);
        result.Slug.Should().Be(dto.Slug);
        result.AuthorId.Should().Be(authorId);
        result.IsPublished.Should().BeFalse();
        _postRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task CreatePostAsync_WithNonExistentAuthor_ThrowsException()
    {
        var dto = new CreatePostDto("Test Post", "Test content with enough characters", "test-post");
        var authorId = Guid.NewGuid();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync((User?)null);

        var act = async () => await _postService.CreatePostAsync(dto, authorId);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Author not found");
    }

    [Fact]
    public async Task CreatePostAsync_WithEmptyTitle_ThrowsDomainException()
    {
        var dto = new CreatePostDto("", "Valid content with enough characters", "valid-slug");
        var authorId = Guid.NewGuid();
        var author = new User("John Doe", "john@example.com", "hash", "Author");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(author);

        var act = async () => await _postService.CreatePostAsync(dto, authorId);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Title cannot be empty*");
    }

    [Fact]
    public async Task CreatePostAsync_WithShortTitle_ThrowsDomainException()
    {
        var dto = new CreatePostDto("AB", "Valid content with enough characters", "valid-slug");
        var authorId = Guid.NewGuid();
        var author = new User("John Doe", "john@example.com", "hash", "Author");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(author);

        var act = async () => await _postService.CreatePostAsync(dto, authorId);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Title must be between 3 and 200 characters");
    }

    [Fact]
    public async Task CreatePostAsync_WithEmptyContent_ThrowsDomainException()
    {
        var dto = new CreatePostDto("Valid Title", "", "valid-slug");
        var authorId = Guid.NewGuid();
        var author = new User("John Doe", "john@example.com", "hash", "Author");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(author);

        var act = async () => await _postService.CreatePostAsync(dto, authorId);

        await act.Should().ThrowAsync<DomainException>().WithMessage("*Content cannot be empty*");
    }

    [Fact]
    public async Task CreatePostAsync_WithShortContent_ThrowsDomainException()
    {
        var dto = new CreatePostDto("Valid Title", "Short", "valid-slug");
        var authorId = Guid.NewGuid();
        var author = new User("John Doe", "john@example.com", "hash", "Author");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(author);

        var act = async () => await _postService.CreatePostAsync(dto, authorId);

        await act.Should().ThrowAsync<DomainException>().WithMessage("Content must have at least 10 characters");
    }

    [Fact]
    public async Task CreatePostAsync_WithValidCategories_LinksCategoriesToPost()
    {
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();
        var category1 = new Category("Tech", "tech", "Technology posts");
        var category2 = new Category("Programming", "programming", "Programming posts");

        var dto = new CreatePostDto(
            "Test Post",
            "Test content with enough characters",
            "test-post",
            [category1Id, category2Id]
        );
        var authorId = Guid.NewGuid();
        var author = new User("John Doe", "john@example.com", "hash", "Author");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(author);

        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(category1Id)).ReturnsAsync(category1);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(category2Id)).ReturnsAsync(category2);

        Post? capturedPost = null;
        _postRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Post>()))
            .Callback<Post>(p => capturedPost = p)
            .ReturnsAsync((Post p) => p);

        var result = await _postService.CreatePostAsync(dto, authorId);

        result.Categories.Should().HaveCount(2);
        result.Categories.Should().Contain(c => c.Name == "Tech");
        result.Categories.Should().Contain(c => c.Name == "Programming");

        capturedPost.Should().NotBeNull();
        capturedPost!.Categories.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreatePostAsync_WithNonExistentCategory_ThrowsException()
    {
        var categoryId = Guid.NewGuid();
        var dto = new CreatePostDto(
            "Test Post",
            "Test content with enough characters",
            "test-post",
            [categoryId]
        );
        var authorId = Guid.NewGuid();
        var author = new User("John Doe", "john@example.com", "hash", "Author");

        _userRepositoryMock.Setup(r => r.GetByIdAsync(authorId)).ReturnsAsync(author);
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        var act = async () => await _postService.CreatePostAsync(dto, authorId);

        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Category with ID {categoryId} not found");
    }

    #endregion

    #region GetPostsAsync Tests

    [Fact]
    public async Task GetPostsAsync_ReturnsPagedResult()
    {
        // Arrange
        var posts = new List<Post>
        {
            CreateTestPost("Post 1", "Content 1 with enough characters", "post-1"),
            CreateTestPost("Post 2", "Content 2 with enough characters", "post-2"),
            CreateTestPost("Post 3", "Content 3 with enough characters", "post-3")
        };

        _postRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10))
            .ReturnsAsync((posts, 3));

        // Act
        var result = await _postService.GetPostsAsync(1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task GetPostsAsync_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var page2Posts = new List<Post>
        {
            CreateTestPost("Post 6", "Content 6 with enough characters", "post-6"),
            CreateTestPost("Post 7", "Content 7 with enough characters", "post-7")
        };

        _postRepositoryMock
            .Setup(r => r.GetPagedAsync(2, 5))
            .ReturnsAsync((page2Posts, 12)); // 12 total posts

        // Act
        var result = await _postService.GetPostsAsync(2, 5);

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(12);
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(5);
        result.TotalPages.Should().Be(3); // Math.Ceiling(12/5) = 3
    }

    [Fact]
    public async Task GetPostsAsync_IncludesAllMetadata()
    {
        // Arrange
        var posts = new List<Post>
        {
            CreateTestPost("Post 1", "Content 1 with enough characters", "post-1")
        };

        _postRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 20))
            .ReturnsAsync((posts, 1));

        // Act
        var result = await _postService.GetPostsAsync(1, 20);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
        result.Page.Should().BeGreaterThan(0);
        result.PageSize.Should().BeGreaterThan(0);
        result.TotalPages.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetPostsAsync_WithInvalidPage_ThrowsException()
    {
        // Arrange & Act
        var act = async () => await _postService.GetPostsAsync(0, 10);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Page must be greater than 0*");
    }

    [Fact]
    public async Task GetPostsAsync_WithInvalidPageSize_ThrowsException()
    {
        // Arrange & Act
        var act = async () => await _postService.GetPostsAsync(1, 0);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*PageSize must be greater than 0*");
    }

    [Fact]
    public async Task GetPostsAsync_WithPageSizeAboveLimit_ThrowsException()
    {
        // Arrange & Act
        var act = async () => await _postService.GetPostsAsync(1, 101);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*PageSize cannot exceed 100*");
    }

    [Fact]
    public async Task GetPostsAsync_WithEmptyResult_ReturnsEmptyPagedResult()
    {
        // Arrange
        _postRepositoryMock
            .Setup(r => r.GetPagedAsync(1, 10))
            .ReturnsAsync((new List<Post>(), 0));

        // Act
        var result = await _postService.GetPostsAsync(1, 10);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    #endregion

    #region GetPostByIdAsync Tests

    [Fact]
    public async Task GetPostByIdAsync_WithValidId_ReturnsPost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var post = CreateTestPost("Test Post", "Test content with enough characters", "test-post");
        typeof(Post).GetProperty("Id")!.SetValue(post, postId);

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);

        // Act
        var result = await _postService.GetPostByIdAsync(postId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(postId);
        result.Title.Should().Be("Test Post");
    }

    [Fact]
    public async Task GetPostByIdAsync_WithNonExistentId_ThrowsException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((Post?)null);

        // Act
        var act = async () => await _postService.GetPostByIdAsync(postId);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Post with ID {postId} not found");
    }

    #endregion

    #region GetPostBySlugAsync Tests

    [Fact]
    public async Task GetPostBySlugAsync_WithValidSlug_ReturnsPost()
    {
        // Arrange
        var post = CreateTestPost("Test Post", "Test content with enough characters", "test-post");
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("test-post")).ReturnsAsync(post);

        // Act
        var result = await _postService.GetPostBySlugAsync("test-post");

        // Assert
        result.Should().NotBeNull();
        result.Slug.Should().Be("test-post");
    }

    [Fact]
    public async Task GetPostBySlugAsync_WithNonExistentSlug_ThrowsException()
    {
        // Arrange
        _postRepositoryMock.Setup(r => r.GetBySlugAsync("non-existent")).ReturnsAsync((Post?)null);

        // Act
        var act = async () => await _postService.GetPostBySlugAsync("non-existent");

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("Post with slug non-existent not found");
    }

    #endregion

    #region DeletePostAsync Tests

    [Fact]
    public async Task DeletePostAsync_WithValidId_DeletesPost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        _postRepositoryMock.Setup(r => r.ExistsAsync(postId)).ReturnsAsync(true);

        // Act
        await _postService.DeletePostAsync(postId);

        // Assert
        _postRepositoryMock.Verify(r => r.DeleteAsync(postId), Times.Once);
    }

    [Fact]
    public async Task DeletePostAsync_WithNonExistentId_ThrowsException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        _postRepositoryMock.Setup(r => r.ExistsAsync(postId)).ReturnsAsync(false);

        // Act
        var act = async () => await _postService.DeletePostAsync(postId);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Post with ID {postId} not found");
    }

    #endregion

    #region UpdatePostAsync Tests

    [Fact]
    public async Task UpdatePostAsync_WithValidData_UpdatesPost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var post = CreateTestPost("Old Title", "Old content with enough characters", "old-slug");
        typeof(Post).GetProperty("Id")!.SetValue(post, postId);

        var dto = new UpdatePostDto("New Title", "New content with enough characters", "new-slug");

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);

        // Act
        var result = await _postService.UpdatePostAsync(postId, dto);

        // Assert
        result.Title.Should().Be("New Title");
        result.Content.Should().Be("New content with enough characters");
        result.Slug.Should().Be("new-slug");
        _postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePostAsync_WithNonExistentId_ThrowsException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var dto = new UpdatePostDto("Title", "Content with enough characters", "slug");
        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((Post?)null);

        // Act
        var act = async () => await _postService.UpdatePostAsync(postId, dto);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Post with ID {postId} not found");
    }

    #endregion

    #region PublishPostAsync Tests

    [Fact]
    public async Task PublishPostAsync_WithValidId_PublishesPost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var post = CreateTestPost("Test Post", "Test content with enough characters", "test-post");
        typeof(Post).GetProperty("Id")!.SetValue(post, postId);

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);

        // Act
        var result = await _postService.PublishPostAsync(postId);

        // Assert
        result.IsPublished.Should().BeTrue();
        result.PublishedAt.Should().NotBeNull();
        _postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task PublishPostAsync_WithNonExistentId_ThrowsException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((Post?)null);

        // Act
        var act = async () => await _postService.PublishPostAsync(postId);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Post with ID {postId} not found");
    }

    #endregion

    #region UnpublishPostAsync Tests

    [Fact]
    public async Task UnpublishPostAsync_WithValidId_UnpublishesPost()
    {
        // Arrange
        var postId = Guid.NewGuid();
        var post = CreateTestPost("Test Post", "Test content with enough characters", "test-post");
        typeof(Post).GetProperty("Id")!.SetValue(post, postId);
        post.Publish();

        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync(post);

        // Act
        var result = await _postService.UnpublishPostAsync(postId);

        // Assert
        result.IsPublished.Should().BeFalse();
        result.PublishedAt.Should().BeNull();
        _postRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Post>()), Times.Once);
    }

    [Fact]
    public async Task UnpublishPostAsync_WithNonExistentId_ThrowsException()
    {
        // Arrange
        var postId = Guid.NewGuid();
        _postRepositoryMock.Setup(r => r.GetByIdAsync(postId)).ReturnsAsync((Post?)null);

        // Act
        var act = async () => await _postService.UnpublishPostAsync(postId);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Post with ID {postId} not found");
    }

    #endregion
}