namespace Taalcafe.Models
{
    public class UserConnectionInfo
    {
        public string userName { get; set; }
        public string connectionId { get; set; }
        public bool inCall { get; set; }

        public UserConnectionInfo(string userName, string connectionId, bool inCall = false) {
            this.userName = userName;
            this.connectionId = connectionId;
            this.inCall = inCall;
        }
    }
}