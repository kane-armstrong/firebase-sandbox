namespace Api.Tests.Setup.Clients
{
    public class FirebaseTokenClientOptions
    {
        /// <remarks>Firebase console -> Project settings -> General tab -> Web API Key</remarks>
        public string ApiKey { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool ReturnSecureToken { get; set; }
    }
}
