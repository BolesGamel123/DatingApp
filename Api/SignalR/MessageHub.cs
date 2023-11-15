using Api.Dtos;
using Api.Entities;
using Api.Extensions;
using Api.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace Api.SignalR
{
    public class MessageHub:Hub
    {
        private readonly IMessageRepository _messageRepo;
         private readonly IUserRepository _userRepo;
          private readonly IMapper _mapper;
           private readonly IHubContext<PresenceHub> _presenceHub;
        public MessageHub(IMessageRepository messageRepo,
         IUserRepository userRepo, IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
          _userRepo = userRepo;
            _messageRepo = messageRepo;
               _mapper=mapper;  
                _presenceHub = presenceHub;
          
        }


       public override async Task OnConnectedAsync()
       {
           var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
             var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
              await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
          var group= await AddToGroup(groupName);

          await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

               var messages = await _messageRepo
                             .GetMessageThread(Context.User.GetUsername(), otherUser);

              await Clients.Caller.SendAsync("ReceiveMessageThread", messages);               
       }



    public override async Task OnDisconnectedAsync(Exception ex)
    {
         var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup");
        await base.OnDisconnectedAsync(ex);
    }

      public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username=Context.User.GetUsername();

            if(username==createMessageDto.RecipientUsername.ToLower())
            {
                 throw new HubException("You cannot send messages to yourself");
            }


            var sender= await _userRepo.GetUserByUsernameAsync(username);

            var recipient=await _userRepo.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if(recipient==null)
            {
                 throw new HubException("Not found user");
            }


            var message=new Message
            {
                 Sender=sender,
                 SenderUsername=sender.UserName,
                 Recipient=recipient,
                 RecipientUsername=recipient.UserName,
                 Content=createMessageDto.Content,
                 
            };

           var groupName = GetGroupName(sender.UserName, recipient.UserName);
            var group = await _messageRepo.GetMessageGroup(groupName);
             if (group.Connections.Any(x => x.Username == recipient.UserName))
             {
               message.DateRead = DateTime.UtcNow;
              }
              else
              {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                    new { username = sender.UserName, knownAs = sender.KnownAs });
             }
                  }

            _messageRepo.AddMessage(message);

            if(await _messageRepo.SaveAllAsync())
            {
                 await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

            } 

            
    }

         private string GetGroupName(string caller, string other)
       {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    
        }


     


     private async Task<Group> AddToGroup(string groupName)
    {
        var group = await _messageRepo.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new Group(groupName);
           _messageRepo.AddGroup(group);
        }

        group.Connections.Add(connection);

        if(await _messageRepo.SaveAllAsync()) return group;

         throw new HubException("Failed to add to group");
    }



    private async Task<Group> RemoveFromMessageGroup()
    {
       var group = await _messageRepo.GetGroupForConnection(Context.ConnectionId);
        var connection = group.Connections
            .FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

       _messageRepo.RemoveConnection(connection);

      if(await _messageRepo.SaveAllAsync()) return group;


       throw new HubException("Failed to remove from group");
    }

    }
}