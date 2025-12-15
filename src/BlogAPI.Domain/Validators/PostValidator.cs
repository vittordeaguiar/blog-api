using BlogAPI.Domain.Entities;
using FluentValidation;

namespace BlogAPI.Domain.Validators;

public class PostValidator : AbstractValidator<Post>
{
    public PostValidator()
    {
        RuleFor(p => p.Title)
            .NotEmpty().WithMessage("Title cannot be empty")
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters");

        RuleFor(p => p.Content)
            .NotEmpty().WithMessage("Content cannot be empty")
            .MinimumLength(10).WithMessage("Content must have at least 10 characters");

        RuleFor(p => p.Slug)
            .NotEmpty().WithMessage("Slug cannot be empty")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must be URL-friendly (lowercase, hyphens, alphanumeric)");

        RuleFor(p => p.AuthorId)
            .NotEmpty().WithMessage("AuthorId cannot be empty");

        RuleFor(p => p.PublishedAt)
            .Null()
            .When(p => !p.IsPublished)
            .WithMessage("PublishedAt must be null when post is not published");
    }
}
