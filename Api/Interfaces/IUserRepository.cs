using Api.Dtos;
using Api.Entities;
using Api.Helpers;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
       
    

       Task<IEnumerable<AppUser>> GetUsersAsync();

       void Update(AppUser user);

       Task<AppUser> GetUserByIdAsync(int id);

       Task<AppUser> GetUserByUsernameAsync(string username);

      Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

       Task<MemberDto> GetMemberByUsernameAsync(string username);

       Task<string> GetUserGender(string username);

       Task<MemberDto> GetMemberAsync(string username, bool isCurrentUser);

        Task<AppUser> GetUserByPhotoId(int photoId);
    }
}