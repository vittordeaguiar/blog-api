using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.API.Controllers;

[ApiController]
[Route("v1/[controller]")]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await categoryService.GetAllAsync();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await categoryService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}