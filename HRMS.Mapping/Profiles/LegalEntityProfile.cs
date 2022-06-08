using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class LegalEntityProfile : Profile
    {
        public LegalEntityProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<LegalEntityModel, LegalEntityViewModel>();
            CreateMap<EntityGenderModel, EntityGenderViewModel>();
            CreateMap<EntityStatusModel, EntityStatusViewModel>();
        }
    }
}
