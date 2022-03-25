using AutoMapper;
using Subby.Core.Entities;
using Subby.Core.Models.Job;
using Subby.Web.New.Models.AdminViewModels;
using Subby.Web.New.Models.AdvertViewModels;

namespace Subby.Web.New.AutoMappings
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