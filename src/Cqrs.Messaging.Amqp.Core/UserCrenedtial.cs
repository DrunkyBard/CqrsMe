namespace Cqrs.Messaging.Configuration.Amqp
{
    public sealed class UserCrenedtial
    {
        public readonly string UserName;
        public readonly string Password;

        public UserCrenedtial(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
    }
}
