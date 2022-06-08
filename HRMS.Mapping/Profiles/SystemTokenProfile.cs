using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemTokenProfile : Profile
    {
        public SystemTokenProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemTokenModel, SystemRefreshTokenViewModel>()
                .ForPath(dest => dest.UserId, opt => opt.MapFrom(src => src.SystemUser.SystemUserId));
            CreateMap<SystemRefreshTokenBindingModel, SystemTokenModel>()
                .ForPath(dest => dest.SystemUser, opt => opt.MapFrom(src =>
                    new SystemUserModel
                    {
                        SystemUserId = src.UserId
                    }));
        }
    }
}
