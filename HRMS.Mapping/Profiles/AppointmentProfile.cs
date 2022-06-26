using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<AppointmentModel, AppointmentViewModel>();
            CreateMap<CreateAppointmentBindingModel, AppointmentModel>()
                .ForPath(dest => dest.Patient, opt => opt.MapFrom(src =>
                    new PatientModel()
                    {
                        PatientId = src.PatientId,
                        IsUser = src.IsUser,
                        SystemUserId = src.SystemUserId,
                        Occupation = src.Occupation,
                        CivilStatus = new CivilStatusModel()
                        {
                            CivilStatusId = src.CivilStatusId
                        },
                        LegalEntity = new LegalEntityModel()
                        {
                            FirstName = src.FirstName,
                            LastName = src.LastName,
                            MiddleName = src.MiddleName,
                            EmailAddress = src.EmailAddress,
                            MobileNumber = src.MobileNumber,
                            BirthDate = src.BirthDate,
                            Gender = new EntityGenderModel() { GenderId = src.GenderId },
                            CompleteAddress = src.CompleteAddress,
                        },
                        SystemRecordManager = new SystemRecordManagerModel() { }
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));

            CreateMap<UpdateAppointmentBindingModel, AppointmentModel>()
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));

            CreateMap<UpdateAppointmentStatusBindingModel, AppointmentModel>()
                .ForPath(dest => dest.AppointmentStatus, opt => opt.MapFrom(src =>
                    new AppointmentStatusModel()
                    {
                        AppointmentStatusId = src.AppointmentStatusId
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
        }
    }
}
