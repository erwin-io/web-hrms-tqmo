using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class SystemUserProfile : Profile
    {
        public SystemUserProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<SystemUserModel, SystemUserViewModel>();
            CreateMap<CreateSystemUserBindingModel, SystemUserModel>()
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
                .ForPath(dest => dest.SystemUserConfig, opt => opt.MapFrom(src =>
                    new SystemUserConfigModel
                    {
                        SystemUser = new SystemUserModel()
                    }))
                .ForPath(dest => dest.LegalEntity.LegalEntityAddress, opt => opt.MapFrom(src => src.LegalEntityAddress))
                .ForPath(dest => dest.SystemUserType, opt => opt.MapFrom(src =>
                    new SystemUserTypeModel
                    {
                        SystemUserTypeId = src.SystemUserTypeId
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()))
                .ForPath(dest => dest.SystemWebAdminUserRoles, opt => opt.MapFrom(src =>
                    new List<SystemWebAdminUserRolesModel>()));

            CreateMap<CreateAccountSystemUserBindingModel, SystemUserModel>()
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
                .ForPath(dest => dest.SystemUserConfig, opt => opt.MapFrom(src =>
                    new SystemUserConfigModel
                    {
                        SystemUser = new SystemUserModel()
                    }))
                .ForPath(dest => dest.SystemUserType, opt => opt.MapFrom(src =>
                    new SystemUserTypeModel()))
                .ForPath(dest => dest.ProfilePicture, opt => opt.MapFrom(src =>
                    new FileModel()));


            CreateMap<CreateWebAccountSystemUserBindingModel, SystemUserModel>()
                .ForPath(dest => dest.LegalEntity, opt => opt.MapFrom(src =>
                    new LegalEntityModel
                    {
                        FirstName = src.FirstName,
                        LastName = src.LastName,
                        MiddleName = src.MiddleName,
                        EmailAddress = src.EmailAddress,
                        MobileNumber = src.MobileNumber,
                        CompleteAddress = src.CompleteAddress,
                        Gender = new EntityGenderModel() { GenderId = src.GenderId },
                    }))
                .ForPath(dest => dest.SystemUserConfig, opt => opt.MapFrom(src =>
                    new SystemUserConfigModel
                    {
                        SystemUser = new SystemUserModel()
                    }))
                .ForPath(dest => dest.SystemUserConfig, opt => opt.MapFrom(src =>
                    new SystemUserConfigModel
                    {
                        SystemUser = new SystemUserModel()
                    }))
                .ForPath(dest => dest.SystemUserType, opt => opt.MapFrom(src =>
                    new SystemUserTypeModel()))
                .ForPath(dest => dest.ProfilePicture, opt => opt.MapFrom(src =>
                    new FileModel()));

            CreateMap<UpdateSystemUserBindingModel, SystemUserModel>()
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
                        Age = 0,
                        Gender = new EntityGenderModel() { GenderId = src.GenderId }
                    }))
                .ForPath(dest => dest.SystemUserConfig, opt => opt.MapFrom(src =>
                    new SystemUserConfigModel
                    {
                        SystemUser = new SystemUserModel()
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()))
                .ForPath(dest => dest.SystemWebAdminUserRoles, opt => opt.MapFrom(src =>
                    new List<SystemWebAdminUserRolesModel>()));


            CreateMap<SystemUserTypeModel, SystemUserTypeViewModel>();
            CreateMap<UpdateSystemUserNameBindingModel, SystemUserModel>()
                .ForPath(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
            CreateMap<UpdateSystemResetPasswordBindingModel, SystemUserModel>()
                .ForPath(dest => dest.Password, opt => opt.MapFrom(src => src.NewPassword))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
            CreateMap<UpdateSystemPasswordBindingModel, SystemUserModel>()
                .ForPath(dest => dest.Password, opt => opt.MapFrom(src => src.NewPassword))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
            CreateMap<SystemUserStatusTrackerBindingModel, SystemUserModel>();
        }
    }
}
