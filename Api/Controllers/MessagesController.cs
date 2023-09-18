using Api.Dtos;
using Api.Entities;
using Api.Extensions;
using Api.Helpers;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{

    [Authorize]
    public class MessagesController:BaseApiController
    {
        private readonly IMessageRepository _messageRepo;
         private readonly IMapper _mapper;
        private readonly IUserRepository _userRepo;  


        public MessagesController(IMessageRepository messageRepo , IMapper mapper,IUserRepository userRepo)
        {
       
            _messageRepo = messageRepo;
           _mapper=mapper;  
            _userRepo = userRepo; 
        }

      

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username=User.GetUsername();

            if(username==createMessageDto.RecipientUsername.ToLower())
            {
                 return BadRequest("You cannot send messages to yourself");
            }


            var sender= await _userRepo.GetUserByUsernameAsync(username);

            var recipient=await _userRepo.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if(recipient==null)
            {
                return NotFound();
            }


            var message=new Message
            {
                 Sender=sender,
                 SenderUsername=sender.UserName,
                 Recipient=recipient,
                 RecipientUsername=recipient.UserName,
                 Content=createMessageDto.Content,
                 
            };


            _messageRepo.AddMessage(message);

            if(await _messageRepo.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

             return BadRequest("Failed to send message");
        }



    [HttpGet]
    public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams )
    {
        
         messageParams.Username=User.GetUsername();
        var messages = await _messageRepo.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, 
        messages.TotalCount, messages.TotalPages));

        return Ok(messages);
        
    }

    
     [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
    {
        
        var currentUsername=User.GetUsername();

        return Ok(await _messageRepo.GetMessageThread(currentUsername,username));
        
    }

      [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id)
    {
        var username = User.GetUsername();

         var message = await _messageRepo.GetMessage(id);

         if (message.SenderUsername != username && message.RecipientUsername != username) 
            return Unauthorized();

             if (message.SenderUsername == username) message.SenderDeleted = true;
              if (message.RecipientUsername == username) message.RecipientDeleted = true;

      if (message.SenderDeleted && message.RecipientDeleted)
        {
            _messageRepo.DeleteMessage(message);
        }

        if(await _messageRepo.SaveAllAsync())  return Ok();

        return BadRequest("Problem deleting the message");
    }


    }
}