using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Validators;

namespace BlogAPI.Domain.Entities;

public class Category
{
    protected Category()
    {
        Name = null!;
        Slug = null!;
        Posts = null!;
    }

    public Category(string name, string slug, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Slug = slug;
        Description = description;
        CreatedAt = DateTime.UtcNow;
        Posts = new List<Post>();

        Validate();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Slug { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ICollection<Post> Posts { get; private set; }

    public void Update(string name, string slug, string? description)
    {
        Name = name;
        Slug = slug;
        Description = description;
        Validate();
    }

    private void Validate()
    {
        var validator = new CategoryValidator();
        var result = validator.Validate(this);

        if (result.IsValid) return;

        var errorMessage = result.Errors.FirstOrDefault()?.ErrorMessage;
        if (errorMessage != null) throw new DomainException(errorMessage);
    }
}
