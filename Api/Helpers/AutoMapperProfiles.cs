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

                 CreateMap<Message, MessageDto>()
.ForMember(d=>d.SenderPhotoUrl,o=>o.MapFrom(s=>s.Sender.Photos.FirstOrDefault(x=>x.IsMain).Url))
.ForMember(d=>d.RecipientPhotoUrl,o=>o.MapFrom(s=>s.Recipient.Photos.FirstOrDefault(x=>x.IsMain).Url));


        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? 
            DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);



          }
    }
}