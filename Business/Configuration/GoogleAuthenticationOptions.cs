namespace Business.Configuration
{
    public class GoogleAuthenticationOptions
    {
        public bool UseGoogleAuth { get; set; }

        public string? CallbackPath { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
