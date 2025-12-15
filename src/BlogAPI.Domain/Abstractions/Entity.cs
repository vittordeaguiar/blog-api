using FluentValidation;
using BlogAPI.Domain.Exceptions;

namespace BlogAPI.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; protected set; }

    protected void Validate<T>(T instance, AbstractValidator<T> validator)
    {
        var result = validator.Validate(instance);
        if (result.IsValid) return;
        var errorMessages = string.Join("; ", result.Errors.Select(e => e.ErrorMessage));
        throw new DomainException(errorMessages);
    }
}