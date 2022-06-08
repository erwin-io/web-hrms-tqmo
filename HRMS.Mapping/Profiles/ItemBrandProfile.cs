using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class ItemBrandProfile : Profile
    {
        public ItemBrandProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<ItemBrandModel, ItemBrandViewModel>();
            CreateMap<ItemBrandBindingModel, ItemBrandModel>()
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
            CreateMap<UpdateItemBrandBindingModel, ItemBrandModel>()
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
        }
    }
}
