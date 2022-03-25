using AutoMapper;
using Subby.Core.Entities;
using Subby.Core.Models.Job;
using Subby.Blazor.Maui.Models.AdminViewModels;
using Subby.Blazor.Maui.Models.AdvertViewModels;

namespace Subby.Blazor.Maui.AutoMappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewAdvertViewModel, Advert>().ReverseMap();
            CreateMap<SponsorViewModel, Sponsor>().ReverseMap();
            CreateMap<UserViewModel, User>().ReverseMap();
            CreateMap<BenefitViewModel, Benefit>().ReverseMap();
        }
    }
}