namespace Sparrow.Parsing.Example.Nude
{
    internal class AccessPermission
    {
        public AccessPermission(string login, string password, bool remember)
        {
            Login = login;
            Password = password;
            Remember = remember;
        }

        public string Login { get; }
        public string Password { get; }
        public bool Remember { get; }
    }
}
