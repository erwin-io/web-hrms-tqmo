using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemWebAdminUserRolesProfile : Profile
    {
        public SystemWebAdminUserRolesProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemWebAdminUserRolesModel, SystemWebAdminUserRolesViewModel>();
            CreateMap<SystemWebAdminUserRolesBindingModel, SystemWebAdminUserRolesViewModel>()
                .ForPath(dest => dest.SystemWebAdminRole, opt => opt.MapFrom(src =>
                    new SystemWebAdminRoleModel() { SystemWebAdminRoleId = src.SystemWebAdminRoleId }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
        }
    }
}
