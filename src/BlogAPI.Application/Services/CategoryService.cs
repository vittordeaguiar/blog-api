using AutoMapper;
using BlogAPI.Application.Common;
using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Interfaces;

namespace BlogAPI.Application.Services;

public class CategoryService(ICategoryRepository categoryRepository, ICacheService cacheService, IMapper mapper) : ICategoryService
{
    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        var cacheKey = CacheKeys.Categories();

        return await cacheService.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                var categories = await categoryRepository.GetAllAsync();
                return mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            },
            TimeSpan.FromMinutes(60)) ?? [];
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var slug = dto.Name.ToLower().Replace(" ", "-");

        var category = new Category(dto.Name, slug, dto.Description);
        await categoryRepository.AddAsync(category);

        await cacheService.RemoveByPatternAsync(CacheKeys.AllCategories());

        return mapper.Map<CategoryResponseDto>(category);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null) throw new DomainException("Category not found");

        await categoryRepository.DeleteAsync(id);

        await cacheService.RemoveByPatternAsync(CacheKeys.AllCategories());
    }
}