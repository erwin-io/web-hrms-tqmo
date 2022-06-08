using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemConfigProfile : Profile
    {
        public SystemConfigProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemConfigModel, SystemConfigViewModel>();
            CreateMap<SystemConfigBindingModel, SystemConfigModel>()
                .ForPath(dest => dest.SystemConfigType, opt => opt.MapFrom(src =>
                    new SystemConfigTypeModel() { SystemConfigTypeId = src.SystemConfigTypeId }));
            CreateMap<UpdateSystemConfigBindingModel, SystemConfigModel>()
                .ForPath(dest => dest.SystemConfigType, opt => opt.MapFrom(src =>
                    new SystemConfigTypeModel() { SystemConfigTypeId = src.SystemConfigTypeId }));
        }
    }
}
