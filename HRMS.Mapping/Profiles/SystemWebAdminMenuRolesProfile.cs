using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemWebAdminMenuRolesProfile : Profile
    {
        public SystemWebAdminMenuRolesProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemWebAdminMenuRolesModel, SystemWebAdminMenuRolesViewModel>();
        }
    }
}
