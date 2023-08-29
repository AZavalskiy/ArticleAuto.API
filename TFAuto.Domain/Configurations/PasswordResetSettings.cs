namespace TFAuto.Domain.Configurations
{
    public class PasswordResetSettings
    {
        public double TokenLifetimeInSeconds { get; set; }

        public int TokenLength { get; set; }

        public string ResetLinkBaseUrl { get; set; }
    }
}