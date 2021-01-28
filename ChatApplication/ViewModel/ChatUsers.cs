using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication.ViewModel
{

    public class ChatUsersResult
    {
        [JsonProperty("Result")]
        public List<ChatUser> Result { get; set; }
    }

    public class ChatUser
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string ProfileImage { get; set; }

        public string ConversationId { get; set; }
    }
}
