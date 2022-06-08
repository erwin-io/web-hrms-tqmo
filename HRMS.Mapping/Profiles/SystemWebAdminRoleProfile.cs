using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemWebAdminRoleProfile : Profile
    {
        public SystemWebAdminRoleProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemWebAdminRoleModel, SystemWebAdminRoleViewModel>();
            CreateMap<SystemWebAdminRoleBindingModel, SystemWebAdminRoleModel>()
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
            CreateMap<UpdateSystemWebAdminRoleBindingModel, SystemWebAdminRoleModel>()
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
        }
    }
}
