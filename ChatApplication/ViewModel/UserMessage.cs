using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApplication.ViewModel
{

    public class UserMessagesResult
    {
        [JsonProperty("Result")]
        public UserMessage Result { get; set; }
    }

    public class UserMessage
    {
        public string UserchatId { get; set; }
        public string ChatconversId { get; set; } // foreign key
        public string Message { get; set; }
        public Guid FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public bool IsSeen { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime SeeOn { get; set; }

    }

    public class MessageHistoriesResult
    {
        [JsonProperty("Result")]
        public MessageHistory Result { get; set; }
    }

    public class MessageHistory
    {
        public List<ChatUser> UserDetail { get; set; }

        public List<UserMessage> Messages { get; set; }
    }
}
