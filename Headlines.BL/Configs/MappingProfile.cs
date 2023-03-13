using AutoMapper;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;

namespace Headlines.BL.Configs
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Article, ArticleDto>();
            CreateMap<ArticleSource, ArticleSourceDto>();

            CreateMap<HeadlineChange, HeadlineChangeDto>();

            CreateMap<ObjectData, ObjectDataDto>();

            CreateMap<UserUpvotes, UserUpvotesDto>();
        }
    }
}