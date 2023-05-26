namespace UnityGit.Core.Data
{
    public struct Credentials
    {
        public string Username;
        public string Password;

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
        }

        public static readonly Credentials InvalidCredentials =
            new Credentials { Username = "", Password = "" };
    }
}
