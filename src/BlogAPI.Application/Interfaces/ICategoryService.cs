using BlogAPI.Application.DTOs;

namespace BlogAPI.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
    Task<CategoryResponseDto> CreateCategoryAsync(CreateCategoryDto dto);
    Task DeleteCategoryAsync(Guid id);
}