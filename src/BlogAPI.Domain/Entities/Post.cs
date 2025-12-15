using BlogAPI.Domain.Exceptions;
using BlogAPI.Domain.Validators;

namespace BlogAPI.Domain.Entities;

public class Post
{
    protected Post()
    {
        Title = null!;
        Content = null!;
        Slug = null!;
        Author = null!;
        Categories = null!;
    }

    public Post(string title, string content, string slug, Guid authorId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Content = content;
        Slug = slug;
        AuthorId = authorId;
        CreatedAt = DateTime.UtcNow;
        IsPublished = false;
        Categories = new List<Category>();

        Validate();
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string Slug { get; private set; }
    public Guid AuthorId { get; private set; }
    public User Author { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public bool IsPublished { get; private set; }
    public ICollection<Category> Categories { get; private set; }

    public void Update(string title, string content, string slug)
    {
        Title = title;
        Content = content;
        Slug = slug;
        UpdatedAt = DateTime.UtcNow;
        Validate();
    }

    public void Publish()
    {
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        IsPublished = false;
        PublishedAt = null;
    }

    public void AddCategory(Category category)
    {
        if (!Categories.Contains(category))
            Categories.Add(category);
    }

    public void RemoveCategory(Category category)
    {
        Categories.Remove(category);
    }

    private void Validate()
    {
        var validator = new PostValidator();
        var result = validator.Validate(this);

        if (result.IsValid) return;

        var errorMessage = result.Errors.FirstOrDefault()?.ErrorMessage;
        if (errorMessage != null) throw new DomainException(errorMessage);
    }
}