using ChatApplication.Controllers;
using ChatApplication.Providers;
using ChatApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication.Hubs
{
    public class ChatHub : Hub
    {

        //private const string DefaultUserImage = "/Images/UsersImage/imgDefault.jpg";
        //private const string RoomId = "1";
        UserMessage usermessage = new UserMessage();
        UserMessagesResult usermessageResult = new UserMessagesResult();
        readonly string baseUrl = "";
        private static IOptions<BaseUrl> _baseurl;
        private readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        HttpRequest Request { get; }


        public ChatHub(IOptions<BaseUrl> baseurl)
        {
            _baseurl = baseurl;
            baseUrl = _baseurl.Value.UrlSetting;
        }

        public override Task OnConnectedAsync( )
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext.Request.Query["UserId"].ToString();
           
            _connections.Add(userId, Context.ConnectionId);

            ////update user's list on client side for online
             UpdateConnectedUsers(userId,"");

            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
             await base.OnDisconnectedAsync(exception);

            var httpContext = Context.GetHttpContext();
            var userId = httpContext.Request.Query["UserId"].ToString();

            _connections.Remove(userId, Context.ConnectionId);

            ////update user's list on client side for offline
            await UpdateConnectedUsers(userId, "Diconnected");

            //return base.OnDisconnectedAsync();
        }

        //public string GetUserList(string roomId, string conversationId)
        //{

        //}

        //public string GetUserInfo(string userId)
        //{

        //}

        public string GetRoomsList()
        {
            return string.Empty;
        }

        //public void SendMessage(string roomId, string conversationId, string otherUserId,
        //    string messageText, string clientGuid)
        //{

        //}

        public async Task SendMessage(Guid fromuser, Guid touser, string ChatconversId, string fromuserimagestring, string message)
        {
            try
            {
                usermessage.FromUserId = fromuser;
                usermessage.ToUserId = touser;
                usermessage.ChatconversId = ChatconversId;
                usermessage.Message = message;
                var requestUri = baseUrl + "UserChat/SaveMessage";
                var response = await HttpRequestFactory.Post(requestUri, usermessage, "");
                if (response.IsSuccessStatusCode)
                {
                    usermessageResult = response.ContentAsType<UserMessagesResult>();
                    ChatconversId = usermessageResult.Result.ChatconversId;
                    TimeSpan ts = DateTime.Now.Subtract(System.Convert.ToDateTime(DateTime.Now));
                    var strdate = (ts.TotalSeconds >= 3600) ? System.Convert.ToDateTime(DateTime.Now).ToString("d MMM yy H:mm tt") : (ts.Hours == 0 ? ts.Minutes + " m ago" : ts.Hours + " h ago");

                    await Clients.All.SendAsync("ReceiveMessage", fromuser, touser, ChatconversId, fromuserimagestring, message, strdate);
                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            
            
        }

        public void SendTypingSignal(string roomId, string conversationId, string userToId)
        {
            //var userId = Context.User.Identity.Name;
            //var users = _context.Users.ToList();
            //var chatUser = (from user in users
            //                where user.UserName == userId
            //                select new ChatModel
            //                {
            //                    Email = user.Email,
            //                    ProfilePictureUrl = (!string.IsNullOrEmpty(user.ImagePath) ?
            //                     File.Exists(Context.Request.GetHttpContext().Server.MapPath(Path.Combine("~", user.ImagePath))) ?
            //                     user.ImagePath : DefaultUserImage : DefaultUserImage),
            //                    Id = user.UserName,
            //                    Name = user.FirstName,
            //                    RoomId = RoomId,
            //                    Status = (_connections.GetConnections(user.Email).Count() == 0 ? "0" : "1")
            //                }).FirstOrDefault();
            //var signal = new ChatSignaling
            //{
            //    ConversationId = null,
            //    RoomId = RoomId,
            //    UserToId = userToId,
            //    UserFrom = chatUser
            //};
            //var host = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            //host.Clients.User(userToId).SendTypingSignal(Newtonsoft.Json.JsonConvert.SerializeObject(signal));
        }

        public string GetMessageHistory(string roomId,
            string conversationId, string otherUserId)
        {
            //roomId = roomId ?? RoomId;
            //var history = (from message in _context.UsersChat
            //               where message.FromUserId == otherUserId ||
            //               message.OtherUserId == otherUserId
            //               orderby message.CreatedOn.Value
            //               select new MessageInfo
            //               {
            //                   ClientGuid = message.ClientGuidId,
            //                   ConversationId = conversationId,
            //                   Message = message.Message,
            //                   RoomId = roomId,
            //                   UserFromId = message.OtherUserId,
            //                   UserToId = message.FromUserId
            //               }).ToList();

            //return Newtonsoft.Json.JsonConvert.SerializeObject(history);
            return null;
        }

        public void EnterRoom(string roomId)
        {
            var y = string.Empty;
        }

        public string LeaveRoom(string roomId)
        {
            return string.Empty;
        }

       

        private async Task UpdateConnectedUsers(string UserId,string onDisconnect = null)
        {

            //if (string.IsNullOrEmpty(onDisconnect) ?
            //    _connections.GetUsers().Count() > 1
            //    : _connections.GetUsers().Count() > 0)
            //{
            //    await Clients.All.SendAsync("UpdateConnectedSignaling", userId);
            //}

            if (string.IsNullOrEmpty(onDisconnect) && _connections.GetUsers().Count() > 0)
            {
                List<string> UserIds = _connections.GetUsers().ToList();
                await Clients.All.SendAsync("UpdateConnectedSignaling", UserIds);
            }
            else if (!string.IsNullOrEmpty(onDisconnect))
            {
                await Clients.All.SendAsync("UpdateDisConnectedSignaling", UserId);
            }
        }

        // OnReconnected() method has deprecated in SignalR core

        //public override Task OnReconnected()
        //{
        //    string name = Context.User.Identity.Name;

        //    if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
        //    {
        //        _connections.Add(name, Context.ConnectionId);
        //    }

        //    //update user's list on client side for online
        //    UpdateConnectedUsers();

        //    return base.OnReconnected();
        //}

    }

   
    
}
