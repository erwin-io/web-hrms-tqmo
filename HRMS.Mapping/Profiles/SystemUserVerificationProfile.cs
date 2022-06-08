using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemUserVerificationProfile : Profile
    {
        public SystemUserVerificationProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemUserVerificationModel, SystemUserVerificationViewModel>();
            CreateMap<SystemUserVerificationBindingModel, SystemUserVerificationModel>();
        }
    }
}
