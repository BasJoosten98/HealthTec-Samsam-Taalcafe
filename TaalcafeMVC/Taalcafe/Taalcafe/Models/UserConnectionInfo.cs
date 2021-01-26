namespace Taalcafe.Models
{
    public class UserConnectionInfo
    {
        public string userName { get; set; }
        public string connectionId { get; set; }
        public bool inCall { get; set; }


        // Use of constructor not possible since client can't use datatype and constructor directly when sending data
        /*
        public UserConnectionInfo(string userName, string connectionId, bool inCall = false) {
            this.userName = userName;
            this.connectionId = connectionId;
            this.inCall = inCall;
        }
        */
    }
}