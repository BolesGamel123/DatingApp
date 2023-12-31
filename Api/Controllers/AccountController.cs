

using System.Security.Cryptography;
using System.Text;
using Api.Data;
using Api.Dtos;
using Api.Entities;
using Api.Interfaces;
using Api.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;


        public AccountController(UserManager<AppUser> userManager,ITokenService tokenService,IMapper mapper)
        {
            _tokenService = tokenService;
            _mapper=mapper;
            _userManager=userManager;
        }


         [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }

            var user=_mapper.Map<AppUser>(registerDto);          
             user.UserName=registerDto.Username.ToLower(); 

         var result= await _userManager.CreateAsync(user,registerDto.Password);

          if (!result.Succeeded) return BadRequest(result.Errors);

          var resultRole= await _userManager.AddToRoleAsync(user,"Member");

          if(!resultRole.Succeeded) return BadRequest(result.Errors);
          

            return new UserDto
            {
                Username=user.UserName,
                Token=await _tokenService.CreateToken(user),
                KnownAs=user.KnownAs,
                Gender=user.Gender
            };
        }

     [HttpPost("Login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user= await _userManager.Users
        .Include(x=>x.Photos)
        .SingleOrDefaultAsync(x=>x.UserName==loginDto.Username);

        if(user==null) return Unauthorized("invalid username");

        var result =await _userManager.CheckPasswordAsync(user,loginDto.Password);

         if (!result) return Unauthorized();

         return new UserDto
            {
                Username=user.UserName,
                Token= await _tokenService.CreateToken(user),
                 PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                 KnownAs=user.KnownAs,
                 Gender=user.Gender
            };

    }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }
    }
}