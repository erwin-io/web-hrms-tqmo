using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemUserConfigProfile : Profile
    {
        public SystemUserConfigProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemUserConfigModel, SystemUserConfigViewModel>();
            CreateMap<UpdateSystemUserConfigBindingModel, SystemUserConfigModel>();
        }
    }
}
