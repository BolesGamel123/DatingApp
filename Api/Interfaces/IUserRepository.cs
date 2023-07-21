using Api.Dtos;
using Api.Entities;

namespace Api.Interfaces
{
    public interface IUserRepository
    {
       
       Task<bool> SaveAllAsync();

       Task<IEnumerable<AppUser>> GetUsersAsync();

       void Update(AppUser user);

       Task<AppUser> GetUserByIdAsync(int id);

       Task<AppUser> GetUserByUsernameAsync(string username);

      Task<IEnumerable<MemberDto>> GetMembersAsync();

       Task<MemberDto> GetMemberByUsernameAsync(string username);

        
    }
}