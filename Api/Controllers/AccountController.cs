

using System.Security.Cryptography;
using System.Text;
using Api.Data;
using Api.Dtos;
using Api.Entities;
using Api.Interfaces;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    public class AccountController:BaseApiController
    {
         private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context,ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;   
        }


         [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username))
            {
                return BadRequest("Username is taken");
            }
            using var hmac=new HMACSHA512();

            var user=new AppUser
            {
                
               UserName=registerDto.Username.ToLower(),
               PasswordHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
               PasswordSalt=hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username=user.UserName,
                Token=_tokenService.CreateToken(user)
            };
        }

     [HttpPost("Login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user= await _context.Users
        .Include(x=>x.Photos)
        .SingleOrDefaultAsync(x=>x.UserName==loginDto.Username);

        if(user==null) return Unauthorized("invalid username");

         using var hmac=new HMACSHA512(user.PasswordSalt);
         var ComputedHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

         for(int i=0 ; i<ComputedHash.Length ; i++)
         {
               if(ComputedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
         }

         return new UserDto
            {
                Username=user.UserName,
                Token=_tokenService.CreateToken(user),
                 PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };

    }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());
        }
    }
}