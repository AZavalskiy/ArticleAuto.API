using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;
using TFAuto.Domain.ServiceConfigurations;
using TFAuto.Domain.Services.Email;
using TFAuto.Domain.Services.PasswordReset;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.ResetPassword
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;        

        public PasswordResetService(
            IRepository<User> userRepository, 
            IMemoryCache memoryCache, 
            IConfiguration configuration,
            EmailService emailService)
        {
            _memoryCache = memoryCache;
            _userRepository = userRepository;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async ValueTask<bool> RequestPasswordResetAsync(string email)
        {                       
            var user = await _userRepository.GetAsync(t => t.Email == email).FirstOrDefaultAsync();

            if (user == null)
                throw new ValidationException(ErrorMessages.INVALID_EMAIL);

            var resetToken = GenerateResetToken();
            var passResetSettings = GetPasswordResetSettings();

            _memoryCache.Set(resetToken, user.Id, TimeSpan.FromMinutes(passResetSettings.TokenExpiryMinutes));

            var resetLink = GeneratePasswordResetLink(resetToken);
            await _emailService.SendPasswordResetEmailAsync(email, resetLink);

            return true;
        }

        public async ValueTask<bool> ResetPasswordAsync(string resetToken, string newPassword)
        {
            if (_memoryCache.TryGetValue(resetToken, out string userId))
            {
                var user = await _userRepository.TryGetAsync(userId, nameof(User));

                if (user != null)
                {
                    user.Password = newPassword;
                    await _userRepository.UpdateAsync(user);
                    _memoryCache.Remove(resetToken);

                    return true;
                }
            }

            return false;
        }

        private string GeneratePasswordResetLink(string resetToken)
        {
            var passResetSettings = GetPasswordResetSettings();
            var resetLink = $"{passResetSettings.ResetLinkBaseUrl}?token={resetToken}";
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