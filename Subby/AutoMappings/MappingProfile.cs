using AutoMapper;
using Subby.Core.Entities;
using Subby.Core.Models.Job;
using Subby.Models.AdminViewModels;
using Subby.Models.AdvertViewModels;

namespace Subby.AutoMappings
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