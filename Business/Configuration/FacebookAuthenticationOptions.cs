namespace Business.Configuration
{
    public class FacebookAuthenticationOptions
    {
        public bool UseFacebookAuth { get; set; }

        public string? AppId { get; set; }
        public string? AppSecret { get; set; }
    }
}
