namespace Business.Configuration
{
    public class TwitterAuthenticationOptions
    {
        public bool UseTwitterAuth { get; set; }

        public string? ConsumerKey { get; set; }
        public string? ConsumerSecret { get; set; }
    }
}
