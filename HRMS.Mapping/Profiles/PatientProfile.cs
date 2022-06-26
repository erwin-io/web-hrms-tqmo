using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<PatientModel, PatientViewModel>();
            CreateMap<CreatePatientBindingModel, PatientModel>()
                .ForPath(dest => dest.LegalEntity, opt => opt.MapFrom(src =>
                    new LegalEntityModel
                    {
                        FirstName = src.FirstName,
                        LastName = src.LastName,
                        MiddleName = src.MiddleName,
                        EmailAddress = src.EmailAddress,
                        MobileNumber = src.MobileNumber,
                        BirthDate = src.BirthDate,
                        CompleteAddress = src.CompleteAddress,
                        Gender = new EntityGenderModel() { GenderId = src.GenderId },
                    }))
                .ForPath(dest => dest.CivilStatus, opt => opt.MapFrom(src =>
                    new CivilStatusModel
                    {
                        CivilStatusId = src.CivilStatusId
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));

            CreateMap<UpdatePatientBindingModel, PatientModel>()
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
                        CompleteAddress = src.CompleteAddress,
                        Age = 0,
                        Gender = new EntityGenderModel() { GenderId = src.GenderId }
                    }))
                .ForPath(dest => dest.CivilStatus, opt => opt.MapFrom(src =>
                    new CivilStatusModel
                    {
                        CivilStatusId = src.CivilStatusId
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
        }
    }
}
