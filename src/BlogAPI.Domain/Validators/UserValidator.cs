using BlogAPI.Domain.Entities;
using FluentValidation;

namespace BlogAPI.Domain.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Name cannot be empty")
            .Length(3, 100).WithMessage("Name must be between 3 and 100 characters");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Invalid Email address")
            .Must(email => !string.IsNullOrEmpty(email) && email.Contains('.') && email.Split('@').Last().Contains('.'))
            .WithMessage("Email must contain a valid domain (e.g. .com)");

        RuleFor(user => user.PasswordHash)
            .NotEmpty().WithMessage("PasswordHash cannot be empty");

        RuleFor(user => user.Role)
            .NotEmpty()
            .Must(role => role is "Admin" or "Author")
            .WithMessage("Invalid Role");
    }
}