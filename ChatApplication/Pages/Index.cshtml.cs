using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApplication.Providers;
using ChatApplication.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace ChatApplication.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public ChatUsersResult chatUsers { get; set; }
        public ChatUsersResult currentUser { get; set; }
        public MessageHistoriesResult messageshistory { get; set; }
        public List<UserMessage> messages { get; set; }
        public List<ChatUser> userlist { get; set; }
        public ChatUser loggedinuser = new ChatUser();
        public ChatUser friend = new ChatUser();
        public List<ChatUser> messageusers = new List<ChatUser>();   //users with whom chat is present
        public List<ChatUser> followusers = new List<ChatUser>(); //suggested users
        public string userid { get; set; }
        readonly string baseUrl = "";
        public string firstconversationId = "";
        private IOptions<BaseUrl> _baseurl;

        public IndexModel(IOptions<BaseUrl> baseurl)
        {
            _baseurl = baseurl;
            baseUrl = _baseurl.Value.UrlSetting;
        }

        public static bool IsGuid(string guidString)
        {
            bool bResult = false;
            try
            {
                Guid g = new Guid(guidString);
                bResult = true;
            }
            catch
            {
                bResult = false;
            }

            return bResult;
        }
        //public async void OnGet(string UserId= "7E9B08D3-6072-4102-B377-08D6E995A6B1", string FriendUserId= "ED026FB8-D78D-40A6-6245-08D6EF194790")
        public async void OnGet(string UserId, string FriendUserId)
        {
            try
            {

                if (!string.IsNullOrEmpty(UserId))
                {
                    userid = UserId;

                }
                else
                {
                    userid = Request.Cookies["FemoriUserId"];
                }
                CreateCookie("FemoriUserId", userid);
                //new Guid(Request.Cookies["FemoriUserId"]); //new Guid("036315E7-8307-4546-44B1-08D6E7F5F21E");



                var messagehistoryuri = baseUrl + "UserChat/GetMessageHistory?currUserId=" + userid + "&friendUserId="+ FriendUserId;
                var messagehistoryresponse = Task.Run(async () => await HttpRequestFactory.Get(messagehistoryuri, "")).Result;
                if (messagehistoryresponse.IsSuccessStatusCode)
                {
                    messageshistory = messagehistoryresponse.ContentAsType<MessageHistoriesResult>();
                }

                List<UserMessage> msg = new List<UserMessage>();
                List<ChatUser> chatuserlist = new List<ChatUser>();
                if (messageshistory != null && messageshistory.Result != null)
                {
                    if (messageshistory.Result.Messages != null)
                    {
                        messages = messageshistory.Result.Messages.OrderByDescending(t => t.CreatedDate).ToList();
                    }
                    if (messageshistory.Result.UserDetail != null)
                    {
                        userlist = messageshistory.Result.UserDetail;
                    }
                        
                }
                else {
                    messages = msg;
                    userlist = chatuserlist;
                }






                var requestUri = baseUrl + "UserChat/GetUserList?UserId=" + userid+ "&FriendUser="+ FriendUserId; //get list of all users who is already in chat list and followerlist
                var response = Task.Run(async () => await HttpRequestFactory.Get(requestUri, "")).Result;

                if (response.IsSuccessStatusCode)
                {
                    chatUsers = response.ContentAsType<ChatUsersResult>();

                    messageusers = chatUsers.Result.Where(t => !string.IsNullOrEmpty(t.ConversationId)).ToList();
                    followusers = chatUsers.Result.Where(t => string.IsNullOrEmpty(t.ConversationId)).ToList();
                    if (!string.IsNullOrEmpty(FriendUserId))
                    {
                        firstconversationId = messageusers.Where(x=>x.UserId == new Guid(FriendUserId)).Select(t => t.ConversationId).FirstOrDefault();
                    }
                    else {
                        firstconversationId = messageusers.Select(t => t.ConversationId).FirstOrDefault();
                    }

                    //if (string.IsNullOrEmpty(firstconversationId))
                    //{
                    //    var createConvoUrl = baseUrl + "UserChat/CreateConversation?UserId1=" + userid + "&UserId2=" + FriendUserId + "";
                    //    var createConvoresponse = Task.Run(async () => await HttpRequestFactory.Get(createConvoUrl, "")).Result;
                    //    if (createConvoresponse.IsSuccessStatusCode)
                    //    {
                    //        messageshistory = createConvoresponse.ContentAsType<MessageHistoriesResult>();
                    //    }
                    //}
                }

                
                //if (messageshistory.Result.Messages != null)
                //{
                //    messages = messageshistory.Result.Messages.OrderByDescending(t => t.CreatedDate).ToList();
                //}
                    
                loggedinuser = userlist.Where(t => t.UserId == new Guid(userid)).SingleOrDefault();
                if(!string.IsNullOrEmpty(FriendUserId))
                {
                    

                    friend = userlist.Where(t => t.UserId != new Guid(userid) && t.UserId== new Guid(FriendUserId)).SingleOrDefault();
                }
                else
                {
                    friend = userlist.Where(t => t.UserId != new Guid(userid)).SingleOrDefault();
                }
                


               

                //var getcurrenturi = baseUrl + "UserChat/GetCurrentUser?UserId=" + userid; //get current user list
                //var getcurrentresponse = Task.Run(async () => await HttpRequestFactory.Get(getcurrenturi, "")).Result;

                //if (getcurrentresponse.IsSuccessStatusCode)
                //{
                //    currentUser = getcurrentresponse.ContentAsType<ChatUsersResult>();

                //}



            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public RedirectResult FemoriLoginRedirect()
        {
            return Redirect("https://femorifrontend.pikateck.com/Account/Login");
        }

        public void CreateCookie(string key, object value)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(2);
            Response.Cookies.Append(key, value.ToString(), option);
        }

    }
}
