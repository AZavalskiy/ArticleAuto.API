using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using TFAuto.Domain.Configurations;
using TFAuto.Domain.Services.Email;
using TFAuto.Domain.Services.UserPassword.DTO;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.UserPassword
{
    public class UserPasswordService : IUserPasswordService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public UserPasswordService(
            IRepository<User> userRepository,
            IMemoryCache memoryCache,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _memoryCache = memoryCache;
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async ValueTask<ForgotPasswordResponse> RequestPasswordResetAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetAsync(t => t.Email == request.Email).FirstOrDefaultAsync();

            if (user == null)
                throw new ValidationException(ErrorMessages.INVALID_EMAIL);

            var passResetSettings = GetPasswordResetSettings();

            var resetToken = GenerateResetToken(passResetSettings.TokenLength);
            var сodeExpiration = DateTime.UtcNow.AddSeconds(passResetSettings.TokenLifetimeInSeconds);

            _memoryCache.Set(resetToken, new TokenInfo { UserId = user.Id, Expiration = сodeExpiration }, сodeExpiration);

            var resetLink = GeneratePasswordResetLink();
            await _emailService.SendPasswordResetEmailAsync(request.Email, resetToken, resetLink);

            var response = new ForgotPasswordResponse { Message = "Email with further instructions has been successfully sent." };
            return response;
        }

        public async ValueTask<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var tokenInfo = _memoryCache.Get<TokenInfo>(request.Token);

            if (tokenInfo == null || tokenInfo.Expiration < DateTime.UtcNow)
                throw new ValidationException(ErrorMessages.INVALID_TOKEN);

            var user = await _userRepository.TryGetAsync(tokenInfo.UserId, nameof(User));

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _userRepository.UpdateAsync(user);
            _memoryCache.Remove(request.Token);

            var response = new ResetPasswordResponse { Message = "Password successfully reset." };
            return response;
        }

        private string GeneratePasswordResetLink()
        {
            var passResetSettings = GetPasswordResetSettings();

            var resetLink = $"{passResetSettings.ResetLinkBaseUrl}";

            return resetLink;
        }

        private string GenerateResetToken(int tokenLength)
        {
            var randomNumber = new byte[tokenLength];
            string refreshToken = "";

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken = Convert.ToBase64String(randomNumber);
            }

            return refreshToken;
        }

        private PasswordResetSettings GetPasswordResetSettings()
        {
            return _configuration.GetSection("PasswordResetSettings").Get<PasswordResetSettings>();
        }
    }
}