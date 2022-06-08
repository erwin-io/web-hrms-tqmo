using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemWebAdminMenuProfile : Profile
    {
        public SystemWebAdminMenuProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemWebAdminMenuModel, SystemWebAdminMenuViewModel>();
            CreateMap<SystemWebAdminModuleModel, SystemWebAdminModuleViewModel>();
        }
    }
}
