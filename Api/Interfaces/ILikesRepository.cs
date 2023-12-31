using Api.Dtos;
using Api.Entities;
using Api.Helpers;

namespace Api.Interfaces
{
    public interface ILikesRepository
    {
    Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
    Task<AppUser> GetUserWithLikes(int userId);
     Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}