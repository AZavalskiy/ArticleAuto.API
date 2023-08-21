using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using TFAuto.Domain.ServiceConfigurations;
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

        public async ValueTask<bool> RequestPasswordResetAsync(ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetAsync(t => t.Email == request.Email).FirstOrDefaultAsync();

            if (user == null)
                throw new ValidationException(ErrorMessages.INVALID_EMAIL);

            var passResetSettings = GetPasswordResetSettings();
            var resetToken = GenerateResetToken();
            var сodeExpiration = DateTime.UtcNow.AddMinutes(passResetSettings.TokenExpiryMinutes);

            _memoryCache.Set(resetToken, new TokenInfo { UserId = user.Id, Expiration = сodeExpiration }, сodeExpiration);

            var resetLink = GeneratePasswordResetLink(resetToken);
            await _emailService.SendPasswordResetEmailAsync(request.Email, resetLink);

            return true;
        }

        public async ValueTask<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var tokenInfo = _memoryCache.Get<TokenInfo>(request.Token);

            if (tokenInfo == null || tokenInfo.Expiration < DateTime.UtcNow)
                throw new ValidationException(ErrorMessages.INVALID_TOKEN);

            var user = await _userRepository.TryGetAsync(tokenInfo.UserId, nameof(User));

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _userRepository.UpdateAsync(user);
            _memoryCache.Remove(request.Token);

            return true;
        }

        private string GeneratePasswordResetLink(string resetToken)
        {
            var passResetSettings = GetPasswordResetSettings();
            var resetLink = $"{passResetSettings.ResetLinkBaseUrl}?resetToken={resetToken}";
            return resetLink;
        }

        private string GenerateResetToken()
        {
            byte[] tokenBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(tokenBytes);
            }
            return Convert.ToBase64String(tokenBytes);
        }

        private PasswordResetSettings GetPasswordResetSettings()
        {
            return _configuration.GetSection("PasswordResetSettings").Get<PasswordResetSettings>();
        }
    }
}