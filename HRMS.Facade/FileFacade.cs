using HRMS.Mapping;
using HRMS.Data.Entity;
using HRMS.Data.Interface;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using HRMS.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;

namespace HRMS.Facade
{
    public class FileFacade : IFileFacade
    {
        private readonly IFileRepositoryRepositoryDAC _fileRepositoryRepositoryDAC;

        #region CONSTRUCTORS
        public FileFacade(IFileRepositoryRepositoryDAC fileRepositoryRepositoryDAC)
        {
            _fileRepositoryRepositoryDAC = fileRepositoryRepositoryDAC ?? throw new ArgumentNullException(nameof(fileRepositoryRepositoryDAC));
        }
        #endregion

        public FileViewModel Find(string id) => AutoMapperHelper<FileModel, FileViewModel>.Map(_fileRepositoryRepositoryDAC.Find(id));
    }
}
