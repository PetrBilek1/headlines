using AutoMapper;
using Headlines.BL.Configs;
using Xunit;

namespace Headlines.BL.Tests.Configs
{
    public sealed class MappingProfileTests
    {
        [Fact]
        public void ValidateMappingConfiguration()
        {
            MapperConfiguration mapperConfig = new MapperConfiguration(
            cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            IMapper mapper = new Mapper(mapperConfig);

            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}