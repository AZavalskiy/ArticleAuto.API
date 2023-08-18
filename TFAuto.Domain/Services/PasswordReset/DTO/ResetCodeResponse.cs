namespace TFAuto.Domain.Services.PasswordReset.DTO
{
    public class ResetCodeResponse
    {
        public string Code { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}