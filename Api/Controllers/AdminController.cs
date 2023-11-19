using Api.Entities;
using Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class AdminController:BaseApiController
    {
         private readonly UserManager<AppUser> _userManager;
         private readonly IUnitOfWork _UOW;
          private readonly IPhotoService _photoService;
         public AdminController(UserManager<AppUser> userManager, IUnitOfWork UOW, IPhotoService photoService)
         {
            _userManager=userManager;
            _UOW = UOW;
               _photoService = photoService;
         }



        [Authorize(Policy = "RequireAdminRole")]
       [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
           var users= await _userManager.Users
                     .OrderBy(u=>u.UserName)
                     .Select(u=>new
                     {
                        u.Id,
                        Username = u.UserName,
                        Roles=u.UserRoles.Select(r=>r.Role.Name).ToList()
                     })
                     .ToListAsync();

                return Ok(users);
        }


       [Authorize(Policy = "RequireAdminRole")]
         [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditRoles(string username, [FromQuery] string roles)
        {
          if (string.IsNullOrEmpty(roles)) return BadRequest("You must select at least one role");

           var selectedRoles = roles.Split(",").ToArray();

           var user= await _userManager.FindByNameAsync(username);

           var UserRoles= await _userManager.GetRolesAsync(user); 

           var result= await _userManager.AddToRolesAsync(user,selectedRoles.Except(UserRoles));

             if (!result.Succeeded) return BadRequest("Failed to add to roles");   

             result =await _userManager.RemoveFromRolesAsync(user,UserRoles.Except(selectedRoles));

              if (!result.Succeeded) return BadRequest("Failed to remove from roles");

              return Ok( await _userManager.GetRolesAsync(user)); 
        }


      
       [Authorize(Policy = "ModeratePhotoRole")]
       [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
         {
            var photos = await
                        _UOW.PhotoRepository.GetUnapprovedPhotos();

             return Ok(photos);
         }

        

          [Authorize(Policy = "ModeratePhotoRole")]
          [HttpPost("reject-photo/{photoId}")]

          public async Task<ActionResult> RejectPhoto(int photoId)
         {
            var photo = await _UOW.PhotoRepository.GetPhotoById(photoId);

            if (photo.PublicId != null)
            {
              var result = await _photoService.DeletePhotoAsync(photo.PublicId);
              if (result.Result == "ok")
                 {
                   _UOW.PhotoRepository.RemovePhoto(photo);
                 }
            }
            else
           {
             _UOW.PhotoRepository.RemovePhoto(photo);
           }

           await _UOW.Complete();

           return Ok();
         }

         [HttpPost("approve-photo/{photoId}")]
         public async Task<ActionResult> ApprovePhoto(int photoId)
          {
            var photo = await _UOW.PhotoRepository.GetPhotoById(photoId);

            if (photo == null) return NotFound("Could not find photo");

            photo.IsApproved = true;

            var user = await _UOW.UserRepository.GetUserByPhotoId(photoId);

            if (!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

            await _UOW.Complete();

            return Ok();
          }
    }
}