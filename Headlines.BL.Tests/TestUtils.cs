using AutoMapper;
using Headlines.BL.Configs;

namespace Headlines.BL.Tests
{
    public static class TestUtils
    {
        public static IMapper GetMapper()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            return new Mapper(mapperConfig);
        }
    }
}