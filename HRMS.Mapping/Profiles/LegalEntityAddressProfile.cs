using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class LegalEntityAddressProfile : Profile
    {
        public LegalEntityAddressProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<LegalEntityAddressModel, LegalEntityAddressViewModel>();
            CreateMap<LegalEntityAddressBindingModel, LegalEntityAddressModel>();
            CreateMap<CreateLegalEntityAddressBindingModel, LegalEntityAddressModel>()
                .ForPath(dest => dest.LegalEntity, opt => opt.MapFrom(src =>
                    new LegalEntityModel
                    {
                        LegalEntityId = src.LegalEntityId
                    }));
            CreateMap<UpdateLegalEntityAddressBindingModel, LegalEntityAddressModel>();
        }
    }
}
