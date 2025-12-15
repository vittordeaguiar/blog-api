using BlogAPI.Domain.Abstractions;
using BlogAPI.Domain.Validators;

namespace BlogAPI.Domain.Entities;

public class Category : Entity
{
    protected Category()
    {
        Posts = new List<Post>();
    }

    public Category(string name, string slug, string? description = null)
    {
        Name = name;
        Slug = slug;
        Description = description;
        Posts = new List<Post>();

        Validate(this, new CategoryValidator());
    }

    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public ICollection<Post> Posts { get; private set; }

    public void Update(string name, string slug, string? description)
    {
        Name = name;
        Slug = slug;
        Description = description;
        UpdatedAt = DateTime.UtcNow;

        Validate(this, new CategoryValidator());
    }
}