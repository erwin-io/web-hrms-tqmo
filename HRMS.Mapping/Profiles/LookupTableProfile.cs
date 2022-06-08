using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class LookupTableProfile : Profile
    {
        public LookupTableProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<LookupModel, LookupViewModel>();
            CreateMap<LookupTableModel, LookupTableViewModel>();
        }
    }
}
