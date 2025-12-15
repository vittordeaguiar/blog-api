using AutoMapper;
using BlogAPI.Application.DTOs;
using BlogAPI.Domain.Entities;

namespace BlogAPI.Application.Mappings;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<Post, PostResponseDto>()
            .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.Name))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));

        CreateMap<Category, CategoryResponseDto>();
    }
}
