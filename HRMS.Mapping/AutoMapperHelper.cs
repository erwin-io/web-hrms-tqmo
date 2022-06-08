using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HRMS.Mapping
{
    public class AutoMapperHelper<TSource, TDestination>
         where TSource : class
         where TDestination : class
    {

        /// <summary>
        /// Specific type object mapping where the destination begins as an empty object
        /// </summary>
        /// <param name="Source">Data source to map the object from</param>
        /// <returns>Returns the mapped object</returns>
        public static TDestination Map(TSource Source)
        {
            return AutoMapperConfig.Mapper.Map<TSource, TDestination>(Source);
        }

        /// <summary>
        /// Specific type object mapping where the destination begins as an object passed to this method.
        /// Use this overload when you want to update an existing entity so that you don't overwrite any persisted data
        /// because they were not part of the viewmodel.
        /// </summary>
        /// <param name="Source">Data source to map the object from</param>
        /// <param name="Destination">Object to map source to</param>
        /// <returns>Returns the mapped object</returns>
        public static TDestination Map(TSource Source, TDestination Destination)
        {


            return AutoMapperConfig.Mapper.Map<TSource, TDestination>(Source, Destination);
        }

        /// <summary>
        /// Collection type object mapping
        /// </summary>
        /// <param name="Source">Data source to map the object from</param>
        /// <returns>Returns the mapped object</returns>
        public static IEnumerable<TDestination> MapList(IEnumerable<TSource> Source)
        {

            if (Source.Count() == 0)
                return new List<TDestination>();
            return AutoMapperConfig.Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(Source);
        }

        /// <summary>
        /// Collection type object mapping
        /// </summary>
        /// <param name="Source">Data source to map the object from</param>
        /// <returns>Returns the mapped object</returns>
        public static ICollection<TDestination> MapList(ICollection<TSource> Source)
        {
            if (Source.Count() == 0)
                return new List<TDestination>();

            return AutoMapperConfig.Mapper.Map<ICollection<TSource>, ICollection<TDestination>>(Source);
        }
    }
    public static class AutoMapperExtension
    {
        private static void IgnoreUnmappedProperties(TypeMap map, IMappingExpression expr)
        {
            foreach (string propName in map.GetUnmappedPropertyNames())
            {
                if (map.SourceType.GetProperty(propName) != null)
                {
                    expr.ForSourceMember(propName, opt => opt.DoNotValidate());
                }
                if (map.DestinationType.GetProperty(propName) != null)
                {
                    expr.ForMember(propName, opt => opt.Ignore());
                }
                if (map.SourceType == null)
                {
                    expr.ForMember(propName, opt => opt.Ignore());
                }
            }
        }

        public static void IgnoreUnmapped(this IProfileExpression profile)
        {
            profile.ForAllMaps(IgnoreUnmappedProperties);
        }

        public static void IgnoreUnmapped(this IProfileExpression profile, Func<TypeMap, bool> filter)
        {
            profile.ForAllMaps((map, expr) =>
            {
                if (filter(map))
                {
                    IgnoreUnmappedProperties(map, expr);
                }
            });
        }

        public static void IgnoreUnmapped(this IProfileExpression profile, Type src, Type dest)
        {
            profile.IgnoreUnmapped((TypeMap map) => map.SourceType == src && map.DestinationType == dest);
        }

        public static void IgnoreUnmapped<TSrc, TDest>(this IProfileExpression profile)
        {
            profile.IgnoreUnmapped(typeof(TSrc), typeof(TDest));
        }
    }
}
