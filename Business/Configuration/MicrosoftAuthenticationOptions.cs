namespace Business.Configuration
{
    public class MicrosoftAuthenticationOptions
    {
        public bool UseMicrosoftAuth { get; set; }

        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
