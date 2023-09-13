using Api.Dtos;
using Api.Entities;
using Api.Helpers;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
       
       Task<bool> SaveAllAsync();

       Task<IEnumerable<AppUser>> GetUsersAsync();

       void Update(AppUser user);

       Task<AppUser> GetUserByIdAsync(int id);

       Task<AppUser> GetUserByUsernameAsync(string username);

      Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);

       Task<MemberDto> GetMemberByUsernameAsync(string username);

        
    }
}