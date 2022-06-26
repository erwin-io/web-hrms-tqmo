using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class DiagnosisProfile : Profile
    {
        public DiagnosisProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<DiagnosisModel, DiagnosisViewModel>();
            CreateMap<CreateDiagnosisBindingModel, DiagnosisModel>()
                .ForPath(dest => dest.Appointment, opt => opt.MapFrom(src =>
                    new AppointmentModel
                    {
                        AppointmentId = src.AppointmentId
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
            CreateMap<UpdateDiagnosisBindingModel, DiagnosisModel>()
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
            CreateMap<UpdateDiagnosisStatusBindingModel, DiagnosisModel>()
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
        }
    }
}
