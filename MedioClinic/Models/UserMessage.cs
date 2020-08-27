namespace MedioClinic.Models
{
    public class UserMessage
    {
        public MessageType MessageType { get; set; }

        public string? Message { get; set; }

        public bool DisplayAsRaw { get; set; }

        public bool Display { get; set; }
    }

    public enum MessageType
    {
        Info,
        Warning,
        Error
    }
}