namespace TFAuto.Domain.Configurations
{
    public class PasswordResetSettings
    {
        public double TokenExpiryMinutes { get; set; }

        public string ResetLinkBaseUrl { get; set; }
    }
}
