namespace TFAuto.Domain.ServiceConfigurations
{
    public class PasswordResetSettings
    {
        public double TokenExpiryMinutes { get; set; }

        public string ResetLinkBaseUrl { get; set; }
    }
}
