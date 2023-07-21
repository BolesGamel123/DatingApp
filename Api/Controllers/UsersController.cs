
using Api.Data;
using Api.Dtos;
using Api.Entities;
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
        public UsersController(IUserRepository userRepo , IMapper mapper)
        {
            _userRepo = userRepo; 
            _mapper=mapper;  
        }
      
         [HttpGet]
         public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
         {
         
              return Ok(await _userRepo.GetMembersAsync());
           
         }

    

         [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
              return await _userRepo.GetMemberByUsernameAsync(username);

           
            
        }
        
    }
}