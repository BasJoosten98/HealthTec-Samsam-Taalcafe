namespace Taalcafe.Models
{
    public class CallOffer
    {
        public UserConnectionInfo Caller { get; set; }
        public UserConnectionInfo Callee { get; set; }

        public CallOffer(UserConnectionInfo caller, UserConnectionInfo callee)
        {
            this.Caller = caller;
            this.Callee = callee;
        }
    }
}