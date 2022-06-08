using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemWebAdminRolePrivilegesProfile : Profile
    {
        public SystemWebAdminRolePrivilegesProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemWebAdminRolePrivilegesModel, SystemWebAdminRolePrivilegesViewModel>();
        }
    }
}
