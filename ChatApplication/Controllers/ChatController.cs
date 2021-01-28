using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApplication.Providers;
using ChatApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ChatApplication.Controllers
{
    public class ChatController : Controller
    {
        readonly string baseUrl = "";
        private IOptions<BaseUrl> _baseurl;
        public MessageHistoriesResult messageshistory { get; set; }
        public List<UserMessage> messages { get; set; }
        public List<ChatUser> userlist { get; set; }
        
        public ChatController(IOptions<BaseUrl> baseurl)
        {
            _baseurl = baseurl;
            baseUrl = _baseurl.Value.UrlSetting;
        }

        public async Task<PartialViewResult> Getmessagehistory(string conversationId)
        {
           var requestUri = baseUrl + "UserChat/GetMessageHistory?conversationId=" + conversationId;
            var response = await HttpRequestFactory.Get(requestUri, "");
            if (response.IsSuccessStatusCode)
            {
                messageshistory = response.ContentAsType<MessageHistoriesResult>();
            }
            ViewBag.CurrentUserId = new Guid(Request.Cookies["FemoriUserId"]);
            messages = messageshistory.Result.Messages;
            userlist = messageshistory.Result.UserDetail;
            ViewBag.Messages = messages;
            ViewBag.UserDetails = userlist;
            return PartialView();
        }

        public string GetUserid()
        {
            return Request.Cookies["FemoriUserId"];
        }
    }
}