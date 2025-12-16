using BlogAPI.Domain.Abstractions;
using BlogAPI.Domain.Validators;

namespace BlogAPI.Domain.Entities;

public class Post : Entity
{
    protected Post()
    {
        Categories = [];
    }

    public Post(string title, string content, string slug, Guid authorId)
    {
        Title = title;
        Content = content;
        Slug = slug;
        AuthorId = authorId;
        IsPublished = false;
        Categories = [];

        Validate(this, new PostValidator());
    }

    public string Title { get; private set; } = null!;
    public string Content { get; private set; } = null!;
    public string Slug { get; private set; } = null!;

    public Guid AuthorId { get; private set; }
    public User? Author { get; private set; }

    public bool IsPublished { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    public ICollection<Category> Categories { get; private set; }

    public void Update(string title, string content, string slug)
    {
        Title = title;
        Content = content;
        Slug = slug;
        UpdatedAt = DateTime.UtcNow;

        Validate(this, new PostValidator());
    }

    public void Publish()
    {
        if (IsPublished) return;
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        if (!IsPublished) return;
        IsPublished = false;
        PublishedAt = null;
    }

    public void AddCategory(Category category)
    {
        if (Categories.All(c => c.Id != category.Id))
        {
            Categories.Add(category);
        }
    }

    public void RemoveCategory(Category category)
    {
        var existing = Categories.FirstOrDefault(c => c.Id == category.Id);
        if (existing != null)
        {
            Categories.Remove(existing);
        }
    }
}