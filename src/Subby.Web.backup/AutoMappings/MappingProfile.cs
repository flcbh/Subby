using AutoMapper;
using Subby.Core.Entities;
using Subby.Core.Models.Job;
using Subby.Web.Models.AdminViewModels;
using Subby.Web.Models.AdvertViewModels;

namespace Subby.Web.AutoMappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NewAdvertViewModel, Advert>().ReverseMap();
            CreateMap<NewJobViewModel, Job>().ReverseMap();
            CreateMap<SponsorViewModel, Sponsor>().ReverseMap();
            CreateMap<UserViewModel, User>().ReverseMap();
            CreateMap<BenefitViewModel, Benefit>().ReverseMap();
            CreateMap<Job, JobModel>()
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Slug))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => src.ContractType))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.ViewCount))
                .ForMember(dest => dest.IsFilled, opt => opt.MapFrom(src => src.IsFilled))
                .ForMember(dest => dest.Applications, opt => opt.MapFrom(src => src.Applications.Count))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.User.UserReviews.Count));
        }
    }
}