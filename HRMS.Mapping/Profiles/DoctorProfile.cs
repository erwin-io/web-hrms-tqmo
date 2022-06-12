using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<DoctorModel, DoctorViewModel>();
            CreateMap<CreateDoctorBindingModel, DoctorModel>()
                .ForPath(dest => dest.LegalEntity, opt => opt.MapFrom(src =>
                    new LegalEntityModel
                    {
                        FirstName = src.FirstName,
                        LastName = src.LastName,
                        MiddleName = src.MiddleName,
                        EmailAddress = src.EmailAddress,
                        MobileNumber = src.MobileNumber,
                        BirthDate = src.BirthDate,
                        Gender = new EntityGenderModel() { GenderId = src.GenderId },
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));

            CreateMap<UpdateDoctorBindingModel, DoctorModel>()
                .ForPath(dest => dest.LegalEntity, opt => opt.MapFrom(src =>
                    new LegalEntityModel
                    {
                        LegalEntityId = src.LegalEntityId,
                        FirstName = src.FirstName,
                        LastName = src.LastName,
                        MiddleName = src.MiddleName,
                        EmailAddress = src.EmailAddress,
                        MobileNumber = src.MobileNumber,
                        BirthDate = src.BirthDate,
                        Age = 0,
                        Gender = new EntityGenderModel() { GenderId = src.GenderId }
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
        }
    }
}
