using Api.Dtos;
using Api.Entities;
using Api.Extensions;
using AutoMapper;

namespace Api.Helpers
{
    public class AutoMapperProfiles:Profile
    {
          public AutoMapperProfiles()
          {
               CreateMap<AppUser, MemberDto>()
               .ForMember(d=>d.PhotoUrl,
               o=>o.MapFrom(s=>s.Photos.FirstOrDefault(x=>x.IsMain==true).Url))
              .ForMember(dest => dest.Age, opt => opt.MapFrom(s => s.DateOfBirth.CalcuateAge()));
               CreateMap<photo, photoDto>();  
                
                CreateMap<MemberUpdateDto,AppUser>();
                CreateMap<RegisterDto,AppUser>();
          }
    }
}