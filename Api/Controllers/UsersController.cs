
using System.Security.Claims;
using Api.Data;
using Api.Dtos;
using Api.Entities;
using Api.Extensions;
using Api.Helpers;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
  
  [Authorize]
    public class UsersController:BaseApiController
    {
       
        private readonly IUserRepository _userRepo;
         private readonly IMapper _mapper;

          private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepo , IMapper mapper,  IPhotoService photoService)
        {
            _userRepo = userRepo; 
            _mapper=mapper;  
            _photoService=photoService;
        }
      
      
         [HttpGet]
         public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
         {
               var CurrentUser= await _userRepo.GetMemberByUsernameAsync(User.GetUsername());

               userParams.CurrentUsername=CurrentUser.UserName;
              
              if(string.IsNullOrEmpty(userParams.Gender))
              {
                userParams.Gender=CurrentUser.Gender=="male"?"female":"male";
              }

              var users= await _userRepo.GetMembersAsync(userParams);

              Response.AddPaginationHeader
              (new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages));

               return Ok(users);

           
         }

    
      
         [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
              return await _userRepo.GetMemberByUsernameAsync(username);

        }


         [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
         

            var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());
            
            if(user==null) return NotFound();

               _mapper.Map(memberUpdateDto,user);
            
            
             if(await _userRepo.SaveAllAsync()) return NoContent();

             return BadRequest("Failed to update user");
        }


    [HttpPost("AddPhoto")]
   public async Task<ActionResult<photoDto>> AddPhoto(IFormFile file)
    {
       
      var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());
      if(user==null) return NotFound();
  
       var result= await _photoService.AddPhotoAsync(file);

       if (result.Error != null) return BadRequest(result.Error.Message);

          var photo = new photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };


         if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if( await _userRepo.SaveAllAsync()) 
        {
           return CreatedAtAction(nameof(GetUser),
           new{username=user.UserName},
           _mapper.Map<photoDto>(photo));
        }  

        return BadRequest("Problem adding photo");
    }



   [HttpPut("SetMainPhoto/{photoId}")]
   public async Task<ActionResult> SetMainPhoto(int photoId)
   {
      var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());
      if(user==null) return NotFound();

      var photo= user.Photos.FirstOrDefault(x=>x.Id==photoId);

      if(photo==null) return NotFound();

     if (photo.IsMain) return BadRequest("This is already your main photo");

     var currentMain=user.Photos.FirstOrDefault(x => x.IsMain);

     if (currentMain != null) currentMain.IsMain = false;

     photo.IsMain = true;
     if( await _userRepo.SaveAllAsync()) return NoContent();
     
      return BadRequest("Problem setting main photo");

   }

 [HttpDelete("DeletePhoto/{photoId}")]
 public async Task<ActionResult> DeletePhoto(int photoId)
 {
       var user = await _userRepo.GetUserByUsernameAsync(User.GetUsername());

      if(user==null) return NotFound();

      var photo= user.Photos.FirstOrDefault(x=>x.Id==photoId);

      if(photo==null) return NotFound();
      if (photo.IsMain) return BadRequest("You cannot delete your main photo");

      if (photo.PublicId != null)
      {
        var result = await _photoService.DeletePhotoAsync(photo.PublicId);
        if (result.Error != null) return BadRequest(result.Error.Message);
      }

        user.Photos.Remove(photo);
         if( await _userRepo.SaveAllAsync()) return Ok();
           return BadRequest("Problem deleting photo");
 }
        
    }
}