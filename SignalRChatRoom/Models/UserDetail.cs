namespace SignalRChatRoom.Models
{
    public class UserDetail
    {
        public UserDetail(string group)
        {
            CurrentGroup = group;
        }

        public UserDetail()
            : this("Lobby")
        {

        }

        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string CurrentGroup { get; set; }
    }
}