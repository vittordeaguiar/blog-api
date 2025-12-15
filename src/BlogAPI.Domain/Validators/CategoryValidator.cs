using BlogAPI.Domain.Entities;
using FluentValidation;

namespace BlogAPI.Domain.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Length(2, 50).WithMessage("Name must be between 2 and 50 characters");

        RuleFor(c => c.Slug)
            .NotEmpty().WithMessage("Slug cannot be empty")
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$")
            .WithMessage("Slug must be URL-friendly (lowercase, hyphens, alphanumeric)");

        RuleFor(c => c.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters")
            .When(c => c.Description != null);
    }
}
