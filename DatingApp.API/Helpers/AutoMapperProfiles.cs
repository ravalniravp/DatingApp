using System.Linq;
using AutoMapper;
using DatingApp.API.DTOS;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UsersForListDTO>()
                .ForMember(dest => dest.PhotoUrl,
                opt =>
                    opt
                        .MapFrom(src =>
                            src.Photos.FirstOrDefault(p => p.isMainp).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(
                    src => src.DateOfBirty.CalculateAge()
                ));
            CreateMap<User, UsersForDetailedDTO>().ForMember(dest => dest.PhotoUrl,
                opt =>
                    opt
                        .MapFrom(src =>
                            src.Photos.FirstOrDefault(p => p.isMainp).Url))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(
                    src => src.DateOfBirty.CalculateAge()
                ));
            CreateMap<Photo, PhotosForDetailedDTO>();
            CreateMap<UserForUpdatDTO,User>();
        }
    }
}
