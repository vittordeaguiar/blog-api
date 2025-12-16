using AutoMapper;
using BlogAPI.Application.Common;
using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Interfaces;

namespace BlogAPI.Application.Services;

public class PostService(
    IPostRepository postRepository,
    IUserRepository userRepository,
    ICategoryRepository categoryRepository,
    ICacheService cacheService,
    IMapper mapper)
    : IPostService
{
    public async Task<PostResponseDto> CreatePostAsync(CreatePostDto dto, Guid authorId)
    {
        var author = await userRepository.GetByIdAsync(authorId);
        if (author is null) throw new DomainException("Author not found");

        var post = new Post(dto.Title, dto.Content, dto.Slug, authorId);

        var categories = new List<Category>();

        if (dto.CategoryIds is not null && dto.CategoryIds.Any())
        {
            foreach (var categoryId in dto.CategoryIds)
            {
                var category = await categoryRepository.GetByIdAsync(categoryId);
                if (category is null) throw new DomainException($"Category with ID {categoryId} not found");

                post.AddCategory(category);
                categories.Add(category);
            }
        }

        var savedPost = await postRepository.AddAsync(post);

        await cacheService.RemoveByPatternAsync(CacheKeys.AllPosts());

        return new PostResponseDto(
            Id: savedPost.Id,
            Title: savedPost.Title,
            Content: savedPost.Content,
            Slug: savedPost.Slug,
            AuthorId: savedPost.AuthorId,
            AuthorName: author.Name,
            CreatedAt: savedPost.CreatedAt,
            UpdatedAt: savedPost.UpdatedAt,
            PublishedAt: savedPost.PublishedAt,
            IsPublished: savedPost.IsPublished,
            Categories: categories.Select(c => new CategoryResponseDto(c.Id, c.Name, c.Slug)).ToList()
        );
    }

    private const int MaxPageSize = 100;

    public async Task<PagedResult<PostResponseDto>> GetPostsAsync(int page, int pageSize)
    {
        if (page <= 0) throw new ArgumentException("Page must be greater than 0", nameof(page));
        if (pageSize <= 0) throw new ArgumentException("PageSize must be greater than 0", nameof(pageSize));
        if (pageSize > MaxPageSize) throw new ArgumentException($"PageSize cannot exceed {MaxPageSize}", nameof(pageSize));

        var cacheKey = CacheKeys.PostsPage(page, pageSize, null, null);

        return await cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                var (posts, totalCount) = await postRepository.GetPagedAsync(page, pageSize);
                var postList = mapper.Map<IEnumerable<PostResponseDto>>(posts);
                return new PagedResult<PostResponseDto>(postList, totalCount, page, pageSize);
            },
            TimeSpan.FromMinutes(5)) ?? new PagedResult<PostResponseDto>([], 0, page, pageSize);
    }

    public async Task<PostResponseDto> GetPostByIdAsync(Guid id)
    {
        var cacheKey = CacheKeys.PostById(id);

        var cached = await cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                var post = await postRepository.GetByIdAsync(id);
                return post is null ? null : mapper.Map<PostResponseDto>(post);
            },
            TimeSpan.FromMinutes(10));

        return cached ?? throw new DomainException($"Post with ID {id} not found");
    }

    public async Task<PostResponseDto> GetPostBySlugAsync(string slug)
    {
        var cacheKey = CacheKeys.PostBySlug(slug);

        var cached = await cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                var post = await postRepository.GetBySlugAsync(slug);
                return post is null ? null : mapper.Map<PostResponseDto>(post);
            },
            TimeSpan.FromMinutes(10));

        return cached ?? throw new DomainException($"Post with slug {slug} not found");
    }

    public async Task<PostResponseDto> UpdatePostAsync(Guid id, UpdatePostDto dto)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post is null) throw new DomainException($"Post with ID {id} not found");

        var oldSlug = post.Slug;
        post.Update(dto.Title, dto.Content, dto.Slug);

        if (dto.CategoryIds is not null)
        {
            foreach (var category in post.Categories.ToList())
            {
                post.RemoveCategory(category);
            }

            foreach (var categoryId in dto.CategoryIds)
            {
                var category = await categoryRepository.GetByIdAsync(categoryId);
                if (category is null) throw new DomainException($"Category with ID {categoryId} not found");

                post.AddCategory(category);
            }
        }

        await postRepository.UpdateAsync(post);

        await cacheService.RemoveByPatternAsync(CacheKeys.AllPosts());
        await cacheService.RemoveAsync(CacheKeys.PostById(id));
        await cacheService.RemoveAsync(CacheKeys.PostBySlug(oldSlug));
        if (oldSlug != dto.Slug)
        {
            await cacheService.RemoveAsync(CacheKeys.PostBySlug(dto.Slug));
        }

        return mapper.Map<PostResponseDto>(post);
    }

    public async Task DeletePostAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post is null) throw new DomainException($"Post with ID {id} not found");

        var slug = post.Slug;

        await postRepository.DeleteAsync(id);

        await cacheService.RemoveByPatternAsync(CacheKeys.AllPosts());
        await cacheService.RemoveAsync(CacheKeys.PostById(id));
        await cacheService.RemoveAsync(CacheKeys.PostBySlug(slug));
    }

    public async Task<PostResponseDto> PublishPostAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post is null) throw new DomainException($"Post with ID {id} not found");

        post.Publish();
        await postRepository.UpdateAsync(post);

        await cacheService.RemoveByPatternAsync(CacheKeys.AllPosts());
        await cacheService.RemoveAsync(CacheKeys.PostById(id));
        await cacheService.RemoveAsync(CacheKeys.PostBySlug(post.Slug));

        return mapper.Map<PostResponseDto>(post);
    }

    public async Task<PostResponseDto> UnpublishPostAsync(Guid id)
    {
        var post = await postRepository.GetByIdAsync(id);
        if (post is null) throw new DomainException($"Post with ID {id} not found");

        post.Unpublish();
        await postRepository.UpdateAsync(post);

        await cacheService.RemoveByPatternAsync(CacheKeys.AllPosts());
        await cacheService.RemoveAsync(CacheKeys.PostById(id));
        await cacheService.RemoveAsync(CacheKeys.PostBySlug(post.Slug));

        return mapper.Map<PostResponseDto>(post);
    }
}