using AutoMapper;
using Headlines.DTO.Entities;
using Headlines.ORM.Core.Entities;

namespace Headlines.BL.Configs
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Article, ArticleDTO>();
            CreateMap<ArticleSource, ArticleSourceDTO>();

            CreateMap<HeadlineChange, HeadlineChangeDTO>();

            CreateMap<ScrapeJob, ScrapeJobDTO>();

            CreateMap<UserUpvotes, UserUpvotesDTO>();
        }
    }
}