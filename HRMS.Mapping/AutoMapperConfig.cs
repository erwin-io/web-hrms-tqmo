using AutoMapper;
using System;
using System.Linq;
using System.Reflection;

namespace HRMS.Mapping
{
    public class AutoMapperConfig
    {
        public static IMapper Mapper;
        public static void Configure(params string[] assemblyNames)
        {
            var config = new MapperConfiguration(cfg => GetConfiguration(cfg, assemblyNames));
            var mapper = config.CreateMapper();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
            Mapper = mapper;
        }
        private static void GetConfiguration(IMapperConfigurationExpression cfg, params string[] assemblyNames)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            if (assemblyNames.Any())
            {
                assemblies = assemblies.Where(a => assemblyNames.Contains(a.GetName().Name)).ToArray();
            }
            foreach (var assembly in assemblies)
            {
                var profiles = assembly.GetTypes().Where(x => x != typeof(Profile) && typeof(Profile).IsAssignableFrom(x));
                foreach (var profile in profiles)
                {
                    cfg.AddProfile((Profile)Activator.CreateInstance(profile));
                }
            }
        }
    }
}
