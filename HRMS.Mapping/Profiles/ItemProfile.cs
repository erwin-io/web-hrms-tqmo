using AutoMapper;
using HRMS.Data.Entity;
using HRMS.Domain.BindingModel;
using HRMS.Domain.ViewModel;
using System.Collections.Generic;

namespace HRMS.Mapping.Profiles
{
    public class ItemProfile : Profile
    {
        public ItemProfile()
        {
            this.IgnoreUnmapped();
            CreateMap<ItemModel, ItemViewModel>();
            CreateMap<ItemBindingModel, ItemModel>()
                .ForPath(dest => dest.ItemType, opt => opt.MapFrom(src =>
                    new ItemTypeModel
                    {
                        ItemTypeId = src.ItemTypeId,
                    }))
                .ForPath(dest => dest.ItemBrand, opt => opt.MapFrom(src =>
                    new ItemBrandModel
                    {
                       ItemBrandId  = src.ItemBrandId,
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
            CreateMap<UpdateItemBindingModel, ItemModel>()
                .ForPath(dest => dest.ItemType, opt => opt.MapFrom(src =>
                    new ItemTypeModel
                    {
                        ItemTypeId = src.ItemTypeId,
                    }))
                .ForPath(dest => dest.ItemBrand, opt => opt.MapFrom(src =>
                    new ItemBrandModel
                    {
                        ItemBrandId = src.ItemBrandId,
                    }))
                .ForPath(dest => dest.SystemRecordManager, opt => opt.MapFrom(src =>
                    new SystemRecordManagerModel()));
        }
    }
}
