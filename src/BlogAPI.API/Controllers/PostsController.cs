using System.Security.Claims;
using BlogAPI.Application.DTOs;
using BlogAPI.Application.Interfaces;
using BlogAPI.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.API.Controllers;

[ApiController]
[Route("api/v1/posts")]
public class PostsController(IPostService postService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await postService.GetPostsAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        try
        {
            var result = await postService.GetPostBySlugAsync(slug);
            return Ok(result);
        }
        catch (DomainException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var authorId))
        {
            return Unauthorized("Invalid User ID in Token");
        }

        var result = await postService.CreatePostAsync(dto, authorId);

        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var currentUserId))
        {
            return Unauthorized();
        }

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        try
        {
            var post = await postService.GetPostByIdAsync(id);
            var isAdmin = userRole == "Admin";
            var isOwner = post.AuthorId == currentUserId;

            if (!isAdmin && !isOwner) return Forbid();

            await postService.UpdatePostAsync(id, dto);
            return NoContent();
        }
        catch (DomainException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var currentUserId)) return Unauthorized();

        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        try
        {
            var post = await postService.GetPostByIdAsync(id);
            var isAdmin = userRole == "Admin";
            var isOwner = post.AuthorId == currentUserId;

            if (!isAdmin && !isOwner) return Forbid();

            await postService.DeletePostAsync(id);
            return NoContent();
        }
        catch (DomainException)
        {
            return NotFound();
        }
    }
}