namespace TFAuto.Domain.ServiceConfigurations
{
    public class SendGridSettings
    {
        public string ApiKey { get; set; }

        public string FromName { get; set; }

        public string FromEmail { get; set; }
    }
}
