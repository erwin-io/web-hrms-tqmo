using HRMS.Mapping;
using HRMS.Data.Entity;
using HRMS.Data.Interface;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using HRMS.Domain.Enumerations;
using HRMS.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using System.IO;

namespace HRMS.Facade
{
    public class SystemUserFacade : ISystemUserFacade
    {
        private readonly ISystemUserRepositoryDAC _systemUserRepository;
        private readonly ISystemUserVerificationRepositoryDAC _systemUserVerificationRepositoryDAC;
        private readonly ISystemUserConfigRepositoryDAC _systemUserConfigRepositoryDAC;
        private readonly ILegalEntityRepository _legalEntityRepository;
        private readonly ILegalEntityAddressRepositoryDAC _legalEntityAddressRepositoryDAC;
        private readonly ISystemWebAdminUserRolesRepositoryDAC _systemWebAdminUserRolesRepositoryDAC;
        private readonly IFileRepositoryRepositoryDAC _fileRepositoryDAC;

        #region CONSTRUCTORS
        public SystemUserFacade(ISystemUserRepositoryDAC systemUserRepository,
            ISystemUserVerificationRepositoryDAC systemUserVerificationRepositoryDAC,
            ISystemUserConfigRepositoryDAC systemUserConfigRepositoryDAC,
            ILegalEntityRepository legalEntityRepository,
            ILegalEntityAddressRepositoryDAC legalEntityAddressRepositoryDAC,
            ISystemWebAdminUserRolesRepositoryDAC systemWebAdminUserRolesRepositoryDAC,
            IFileRepositoryRepositoryDAC fileRepositoryDAC)
        {
            _systemUserRepository = systemUserRepository ?? throw new ArgumentNullException(nameof(systemUserRepository));
            _systemUserVerificationRepositoryDAC = systemUserVerificationRepositoryDAC ?? throw new ArgumentNullException(nameof(systemUserVerificationRepositoryDAC));
            _systemUserConfigRepositoryDAC = systemUserConfigRepositoryDAC ?? throw new ArgumentNullException(nameof(systemUserConfigRepositoryDAC));
            _legalEntityRepository = legalEntityRepository ?? throw new ArgumentNullException(nameof(legalEntityRepository));
            _legalEntityAddressRepositoryDAC = legalEntityAddressRepositoryDAC ?? throw new ArgumentNullException(nameof(legalEntityAddressRepositoryDAC));
            _systemWebAdminUserRolesRepositoryDAC = systemWebAdminUserRolesRepositoryDAC ?? throw new ArgumentNullException(nameof(systemWebAdminUserRolesRepositoryDAC));
            _fileRepositoryDAC = fileRepositoryDAC ?? throw new ArgumentNullException(nameof(fileRepositoryDAC));
        }
        #endregion

        public string Add(CreateSystemUserBindingModel model, string CreatedBy)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<CreateSystemUserBindingModel, SystemUserModel>.Map(model);

                    //Start Saving LegalEntity
                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    var legalEntityId = _legalEntityRepository.Add(addModel.LegalEntity);
                    addModel.LegalEntity.LegalEntityId = legalEntityId;
                    //End Saving LegalEntity

                    //Start Saving LegalEntity Address
                    foreach (var addess in addModel.LegalEntity.LegalEntityAddress)
                    {
                        var legalEntityAddressId = _legalEntityAddressRepositoryDAC.Add(new LegalEntityAddressModel()
                        {
                            LegalEntity = new LegalEntityModel() {  LegalEntityId = legalEntityId },
                            Address = addess.Address
                        });
                        if (string.IsNullOrEmpty(legalEntityAddressId))
                        {
                            throw new Exception("Error Saving System User Legal Entity Address");
                        }
                    }
                    //End Saving LegalEntity Address

                    //Start Saving file
                    addModel.ProfilePicture.FileContent = System.Convert.FromBase64String(model.ProfilePicture.FileFromBase64String);
                    addModel.ProfilePicture.SystemRecordManager.CreatedBy = CreatedBy;
                    var fileId = _fileRepositoryDAC.Add(addModel.ProfilePicture);
                    if (string.IsNullOrEmpty(fileId))
                        throw new Exception("Error Saving File");
                    addModel.ProfilePicture.FileId = fileId;
                    //End Saving file

                    //start store file directory
                    if (!model.ProfilePicture.IsDefault)
                    {
                        if (File.Exists(addModel.ProfilePicture.FileName))
                        {
                            File.Delete(addModel.ProfilePicture.FileName);
                        }
                        using (var fs = new FileStream(addModel.ProfilePicture.FileName, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(addModel.ProfilePicture.FileContent, 0, addModel.ProfilePicture.FileContent.Length);
                        }
                    }
                    //end store file directory

                    addModel.SystemRecordManager.CreatedBy = CreatedBy;
                    addModel.LegalEntity.LegalEntityId = legalEntityId;
                    id = _systemUserRepository.Add(addModel);
                    if (string.IsNullOrEmpty(id))
                    {
                        throw new Exception("Error Creating System User");
                    }
                    foreach (var role in model.SystemWebAdminUserRoles)
                    {
                        var SystemWebAdminUserRoleId = _systemWebAdminUserRolesRepositoryDAC.Add(new SystemWebAdminUserRolesModel()
                        {
                            SystemUser = new SystemUserModel() { SystemUserId = id },
                            SystemWebAdminRole = new SystemWebAdminRoleModel() { SystemWebAdminRoleId = role.SystemWebAdminRoleId },
                            SystemRecordManager = new SystemRecordManagerModel() { CreatedBy = CreatedBy }
                        });
                        if (string.IsNullOrEmpty(SystemWebAdminUserRoleId))
                        {
                            throw new Exception("Error Creating System User Roles");
                        }
                    }
                    addModel.SystemUserConfig.SystemUser.SystemUserId = id;
                    addModel.SystemUserConfig.IsUserEnable = true;
                    var configId = _systemUserConfigRepositoryDAC.Add(addModel.SystemUserConfig);
                    if (string.IsNullOrEmpty(configId))
                    {
                        throw new Exception("Error Creating System User Settings");
                    }
                    scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CreateAccount(CreateAccountSystemUserBindingModel model, FileBindingModel profilePic)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<CreateAccountSystemUserBindingModel, SystemUserModel>.Map(model);

                    //Start Saving LegalEntity
                    addModel.LegalEntity.BirthDate = DateTime.Now;
                    var legalEntityId = _legalEntityRepository.Add(addModel.LegalEntity);
                    if (string.IsNullOrEmpty(legalEntityId))
                    {
                        throw new Exception("Error Creating System User Legal Entity");
                    }
                    //End Saving LegalEntity

                    //Start Saving file
                    addModel.ProfilePicture = new FileModel()
                    {
                        FileName = profilePic.FileName,
                        MimeType = profilePic.MimeType,
                        FileContent = profilePic.FileContent,
                        SystemRecordManager = new SystemRecordManagerModel() { }
                    };

                    var fileId = _fileRepositoryDAC.Add(addModel.ProfilePicture);
                    if (string.IsNullOrEmpty(fileId))
                        throw new Exception("Error Saving File");
                    addModel.ProfilePicture.FileId = fileId;
                    //End Saving file

                    //start store file directory
                    if (!profilePic.IsDefault)
                    {
                        if (File.Exists(addModel.ProfilePicture.FileName))
                        {
                            File.Delete(addModel.ProfilePicture.FileName);
                        }
                        using (var fs = new FileStream(addModel.ProfilePicture.FileName, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(addModel.ProfilePicture.FileContent, 0, addModel.ProfilePicture.FileContent.Length);
                        }
                    }
                    //end store file directory

                    //Start Saving User
                    addModel.UserName = model.VerificationSender;
                    addModel.LegalEntity.LegalEntityId = legalEntityId;
                    addModel.SystemUserType.SystemUserTypeId = (int)SYSTEM_USER_TYPE_ENUMS.USER_PORTAL;
                    id = _systemUserRepository.CreateAccount(addModel);
                    if (string.IsNullOrEmpty(id))
                    {
                        throw new Exception("Error Creating System User");
                    }
                    //End Saving LegalEntity

                    //Start Saving User Config
                    addModel.SystemUserConfig.SystemUser.SystemUserId = id;
                    addModel.SystemUserConfig.IsUserEnable = true;
                    var configId = _systemUserConfigRepositoryDAC.Add(addModel.SystemUserConfig);
                    if (string.IsNullOrEmpty(configId))
                    {
                        throw new Exception("Error Creating System User Settings");
                    }
                    //End Saving User Config

                    //Start Saving User Verification
                    var verification = _systemUserVerificationRepositoryDAC.FindBySender(model.VerificationSender, model.VerificationCode);
                    if (!_systemUserVerificationRepositoryDAC.VerifyUser(verification.Id))
                    {
                        throw new Exception("Error Verifying User");
                    }
                    //End Saving User Verification
                    scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CreateWebAdminAccount(CreateAccountSystemUserBindingModel model, FileBindingModel profilePic)
        {
            try
            {
                var id = string.Empty;
                using (var scope = new TransactionScope())
                {
                    var addModel = AutoMapperHelper<CreateAccountSystemUserBindingModel, SystemUserModel>.Map(model);

                    //Start Saving LegalEntity
                    addModel.LegalEntity.BirthDate = DateTime.Now;
                    var legalEntityId = _legalEntityRepository.Add(addModel.LegalEntity);
                    if (string.IsNullOrEmpty(legalEntityId))
                    {
                        throw new Exception("Error Creating System User Legal Entity");
                    }
                    //End Saving LegalEntity

                    //Start Saving file
                    addModel.ProfilePicture = new FileModel()
                    {
                        FileName = profilePic.FileName,
                        MimeType = profilePic.MimeType,
                        FileContent = profilePic.FileContent,
                        SystemRecordManager = new SystemRecordManagerModel() { }
                    };

                    var fileId = _fileRepositoryDAC.Add(addModel.ProfilePicture);
                    if (string.IsNullOrEmpty(fileId))
                        throw new Exception("Error Saving File");
                    addModel.ProfilePicture.FileId = fileId;
                    //End Saving file

                    //start store file directory
                    if (!profilePic.IsDefault)
                    {
                        if (File.Exists(addModel.ProfilePicture.FileName))
                        {
                            File.Delete(addModel.ProfilePicture.FileName);
                        }
                        using (var fs = new FileStream(addModel.ProfilePicture.FileName, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(addModel.ProfilePicture.FileContent, 0, addModel.ProfilePicture.FileContent.Length);
                        }
                    }
                    //end store file directory

                    //Start Saving User
                    addModel.UserName = model.VerificationSender;
                    addModel.LegalEntity.LegalEntityId = legalEntityId;
                    addModel.SystemUserType.SystemUserTypeId = (int)SYSTEM_USER_TYPE_ENUMS.USER_WEBADMIN;
                    addModel.IsWebAdminGuestUser = true;
                    id = _systemUserRepository.CreateAccount(addModel);
                    if (string.IsNullOrEmpty(id))
                    {
                        throw new Exception("Error Creating System User");
                    }
                    //End Saving User

                    //Start Saving User Config
                    addModel.SystemUserConfig.SystemUser.SystemUserId = id;
                    addModel.SystemUserConfig.IsUserEnable = true;
                    var configId = _systemUserConfigRepositoryDAC.Add(addModel.SystemUserConfig);
                    if (string.IsNullOrEmpty(configId))
                    {
                        throw new Exception("Error Creating System User Settings");
                    }
                    //End Saving User

                    //Start Saving User Verification
                    var verification = _systemUserVerificationRepositoryDAC.FindBySender(model.VerificationSender, model.VerificationCode);
                    if (!_systemUserVerificationRepositoryDAC.VerifyUser(verification.Id))
                    {
                        throw new Exception("Error Verifying User");
                    }
                    //End Saving User Verification
                    scope.Complete();
                }
                return id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public PageResultsViewModel<SystemUserViewModel> GetPage(string Search, long SystemUserType, long ApprovalStatus, long PageNo, long PageSize, string OrderColumn, string OrderDir)
        {
            var result = new PageResultsViewModel<SystemUserViewModel>();
            var data = _systemUserRepository.GetPage(Search, SystemUserType, ApprovalStatus, PageNo, PageSize, OrderColumn, OrderDir);
            result.Items = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.MapList(data);
            foreach (var item in result.Items)
            {
                if (item.ProfilePicture != null && File.Exists(item.ProfilePicture.FileName))
                    item.ProfilePicture.FileContent = System.IO.File.ReadAllBytes(item.ProfilePicture.FileName);
            }
            result.TotalRows = data.Count > 0 ? data.FirstOrDefault().PageResult.TotalRows : 0;
            return result;
        }
        public SystemUserViewModel Find(string id)
        {
            var result = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.Find(id));
            if (result.ProfilePicture != null && File.Exists(result.ProfilePicture.FileName))
                result.ProfilePicture.FileContent = System.IO.File.ReadAllBytes(result.ProfilePicture.FileName);
            return result;
        }
        public SystemUserViewModel GetTrackerStatus(string id) => AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.GetTrackerStatus(id));
        public SystemUserViewModel FindByUsername(string Username)
        {
            var result = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.FindByUsername(Username));
            if (result != null && result.ProfilePicture != null && File.Exists(result.ProfilePicture.FileName))
                result.ProfilePicture.FileContent = System.IO.File.ReadAllBytes(result.ProfilePicture.FileName);
            return result;
        }
        public SystemUserViewModel Find(string Username, string Password)
        {
            var result = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.Find(Username, Password));
            if (result != null && result.ProfilePicture != null && File.Exists(result.ProfilePicture.FileName))
                result.ProfilePicture.FileContent = System.IO.File.ReadAllBytes(result.ProfilePicture.FileName);
            return result;
        }
        public bool Remove(string id, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _systemUserRepository.Remove(id, LastUpdatedBy);
                if(success)
                    scope.Complete();
            }
            return success;
        }

        public bool Update(UpdateSystemUserBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateSystemUserBindingModel, SystemUserModel>.Map(model);
                var user = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.Find(updateModel.SystemUserId));
                //Start Saving LegalEntity
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                updateModel.LegalEntity.LegalEntityId = user.LegalEntity.LegalEntityId;
                success = _legalEntityRepository.Update(updateModel.LegalEntity);
                if (!success)
                {
                    throw new Exception("Error Updating System User");
                }
                //End Saving file

                //Start Saving file
                if(model.ProfilePicture != null)
                {
                    if (model.ProfilePicture.IsDefault)
                    {
                        if(!string.IsNullOrEmpty(model.ProfilePicture.FileFromBase64String))
                        {
                            updateModel.ProfilePicture.FileContent = System.Convert.FromBase64String(model.ProfilePicture.FileFromBase64String);
                        }
                        updateModel.ProfilePicture.SystemRecordManager.CreatedBy = LastUpdatedBy;
                        var fileId = _fileRepositoryDAC.Add(updateModel.ProfilePicture);
                        if (string.IsNullOrEmpty(fileId))
                            throw new Exception("Error Saving File");
                        updateModel.ProfilePicture.FileId = fileId;
                    }
                    else
                    {
                        updateModel.ProfilePicture.FileContent = System.Convert.FromBase64String(model.ProfilePicture.FileFromBase64String);
                        updateModel.ProfilePicture.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                        success = _fileRepositoryDAC.Update(updateModel.ProfilePicture);
                        if (!success)
                        {
                            throw new Exception("Error Saving File");
                        }
                    }
                    if (model.ProfilePicture.IsFromStorage)
                    {
                        //start store file directory
                        if (File.Exists(updateModel.ProfilePicture.FileName))
                        {
                            File.Delete(updateModel.ProfilePicture.FileName);
                        }
                        try
                        {
                            using (var fs = new FileStream(updateModel.ProfilePicture.FileName, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(updateModel.ProfilePicture.FileContent, 0, updateModel.ProfilePicture.FileContent.Length);
                            }
                        }
                        catch { }
                    }
                    //end store file directory
                }
                //End Saving file


                var currentSystemWebAdminUserRoles = AutoMapperHelper<SystemWebAdminUserRolesModel, SystemWebAdminUserRolesViewModel>.MapList(_systemWebAdminUserRolesRepositoryDAC.FindBySystemUserId(model.SystemUserId));
                var newSystemWebAdminUserRoles = new List<SystemWebAdminUserRolesModel>();

                foreach (var role in currentSystemWebAdminUserRoles)
                {
                    if(!model.SystemWebAdminUserRoles.Any(swaur=>swaur.SystemWebAdminRoleId == role.SystemWebAdminRole.SystemWebAdminRoleId))
                    {
                        var systemUserRole = _systemWebAdminUserRolesRepositoryDAC.FindBySystemWebAdminRoleIdAndSystemUserId(role.SystemWebAdminRole.SystemWebAdminRoleId, model.SystemUserId);
                        if(systemUserRole != null)
                        {
                            if (!_systemWebAdminUserRolesRepositoryDAC.Remove(systemUserRole.SystemWebAdminUserRoleId, LastUpdatedBy))
                                throw new Exception("Error Updating System User Role");
                        }
                    }
                }
                foreach (var role in model.SystemWebAdminUserRoles)
                {
                    if (!currentSystemWebAdminUserRoles.Any(swaur => swaur.SystemWebAdminRole.SystemWebAdminRoleId == role.SystemWebAdminRoleId))
                    {
                        var SystemWebAdminUserRoleId = _systemWebAdminUserRolesRepositoryDAC.Add(new SystemWebAdminUserRolesModel()
                        {
                            SystemUser = new SystemUserModel() { SystemUserId = model.SystemUserId },
                            SystemWebAdminRole = new SystemWebAdminRoleModel() { SystemWebAdminRoleId = role.SystemWebAdminRoleId },
                            SystemRecordManager = new SystemRecordManagerModel() { CreatedBy = LastUpdatedBy }
                        });
                        if (string.IsNullOrEmpty(SystemWebAdminUserRoleId))
                        {
                            throw new Exception("Error Creating System User Roles");
                        }
                    }
                }
                if (user.IsWebAdminGuestUser)
                {
                    if(model.SystemWebAdminUserRoles.Count > 0)
                    {
                        updateModel.IsWebAdminGuestUser = false;
                    }
                }
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _systemUserRepository.Update(updateModel);
                if (!success)
                {
                    throw new Exception("Error Updating System User");
                }

                scope.Complete();
            }
            return success;
        }

        public bool UpdatePersonalDetails(UpdateSystemUserBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateSystemUserBindingModel, SystemUserModel>.Map(model);
                var user = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.Find(updateModel.SystemUserId));
                //Start Saving LegalEntity
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                updateModel.LegalEntity.LegalEntityId = user.LegalEntity.LegalEntityId;
                success = _legalEntityRepository.Update(updateModel.LegalEntity);
                if (!success)
                {
                    throw new Exception("Error Updating Personal Details");
                }
                //End Saving file

                //Start Saving file
                if (model.ProfilePicture.IsDefault)
                {
                    updateModel.ProfilePicture.FileContent = System.Convert.FromBase64String(model.ProfilePicture.FileFromBase64String);
                    updateModel.ProfilePicture.SystemRecordManager.CreatedBy = LastUpdatedBy;
                    var fileId = _fileRepositoryDAC.Add(updateModel.ProfilePicture);
                    if (string.IsNullOrEmpty(fileId))
                        throw new Exception("Error Saving File");
                    updateModel.ProfilePicture.FileId = fileId;
                }
                else
                {
                    updateModel.ProfilePicture.FileContent = System.Convert.FromBase64String(model.ProfilePicture.FileFromBase64String);
                    updateModel.ProfilePicture.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                    success = _fileRepositoryDAC.Update(updateModel.ProfilePicture);
                    if (!success)
                    {
                        throw new Exception("Error Saving File");
                    }
                }
                //End Saving file

                //start store file directory
                if (File.Exists(updateModel.ProfilePicture.FileName))
                {
                    File.Delete(updateModel.ProfilePicture.FileName);
                }
                using (var fs = new FileStream(updateModel.ProfilePicture.FileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(updateModel.ProfilePicture.FileContent, 0, updateModel.ProfilePicture.FileContent.Length);
                }
                //end store file directory

                updateModel.IsWebAdminGuestUser = user.IsWebAdminGuestUser;
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _systemUserRepository.Update(updateModel);

                scope.Complete();
            }
            return success;
        }

        public bool UpdateUsername(UpdateSystemUserNameBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateSystemUserNameBindingModel, SystemUserModel>.Map(model);
                var user = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.Find(updateModel.SystemUserId));
                updateModel.IsWebAdminGuestUser = user.IsWebAdminGuestUser;
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _systemUserRepository.UpdateUsername(updateModel);
                if (!success)
                {
                    throw new Exception("Error Updating Username");
                }

                scope.Complete();
            }
            return success;
        }

        public bool UpdatePassword(UpdateSystemPasswordBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateSystemPasswordBindingModel, SystemUserModel>.Map(model);
                var user = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.Find(updateModel.SystemUserId));
                updateModel.IsWebAdminGuestUser = user.IsWebAdminGuestUser;
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _systemUserRepository.UpdatePassword(updateModel);
                if (!success)
                {
                    throw new Exception("Error Updating Password");
                }

                scope.Complete();
            }
            return success;
        }

        public bool ResetPassword(UpdateSystemResetPasswordBindingModel model, string LastUpdatedBy)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                var updateModel = AutoMapperHelper<UpdateSystemResetPasswordBindingModel, SystemUserModel>.Map(model);
                var user = AutoMapperHelper<SystemUserModel, SystemUserViewModel>.Map(_systemUserRepository.Find(updateModel.SystemUserId));
                updateModel.IsWebAdminGuestUser = user.IsWebAdminGuestUser;
                updateModel.SystemRecordManager.LastUpdatedBy = LastUpdatedBy;
                success = _systemUserRepository.UpdatePassword(updateModel);
                if (!success)
                {
                    throw new Exception("Error Updating Password");
                }

                scope.Complete();
            }
            return success;
        }

        public bool UpdateSystemUserConfig(UpdateSystemUserConfigBindingModel model)
        {
            var success = false;
            using (var scope = new TransactionScope())
            {
                success = _systemUserConfigRepositoryDAC.Update(AutoMapperHelper<UpdateSystemUserConfigBindingModel, SystemUserConfigModel>.Map(model));
                scope.Complete();
            }
            return success;
        }
    }
}
