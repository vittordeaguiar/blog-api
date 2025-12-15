using AutoMapper;
using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using BlogAPI.Domain.Entities;
using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Interfaces;

namespace BlogAPI.Application.Services;

public class CategoryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryService
{
    public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
    {
        var categories = await categoryRepository.GetAllAsync();
        return mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var slug = dto.Name.ToLower().Replace(" ", "-");

        var category = new Category(dto.Name, slug, dto.Description);
        await categoryRepository.AddAsync(category);

        return mapper.Map<CategoryResponseDto>(category);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        if (category is null) throw new DomainException("Category not found");

        await categoryRepository.DeleteAsync(id);
    }
}