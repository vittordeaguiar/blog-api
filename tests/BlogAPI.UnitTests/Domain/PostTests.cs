using BlogAPI.Domain.Entities;
using FluentAssertions;

namespace BlogAPI.UnitTests.Domain;

public class PostTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePost()
    {
        var post = new Post("Titulo", "Conteudo longo o suficiente", "slug-valido", Guid.NewGuid());

        post.Id.Should().NotBeEmpty();
        post.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        post.IsPublished.Should().BeFalse();
    }

    [Fact]
    public void Publish_ShouldSetIsPublishedTrue_AndSetDate()
    {
        var post = new Post("Titulo", "Conteúdo do Post", "slug", Guid.NewGuid());

        post.Publish();

        post.IsPublished.Should().BeTrue();
        post.PublishedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddCategory_ShouldNotAddDuplicateCategory()
    {
        var post = new Post("Titulo", "Conteúdo do Post", "slug", Guid.NewGuid());
        var category = new Category("Tech", "tech");

        post.AddCategory(category);
        post.AddCategory(category);

        post.Categories.Should().HaveCount(1);
    }

    // TODO: Adicionar mais testes para validações (Título vazio, etc)
}