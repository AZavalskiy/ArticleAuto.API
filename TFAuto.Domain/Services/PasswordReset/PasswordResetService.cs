using Microsoft.Azure.CosmosRepository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
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
        private readonly EmailService _emailService;
        private readonly PasswordResetSettings _resetSettings;

        public PasswordResetService(IMemoryCache memoryCache,
            IRepository<User> userRepository,
            IOptions<PasswordResetSettings> resetSettings,
            EmailService emailService)
        {
            _memoryCache = memoryCache;
            _userRepository = userRepository;
            _resetSettings = resetSettings.Value;
            _emailService = emailService;
        }

        public async ValueTask<bool> RequestPasswordResetAsync(string email)
        {
            //var user = await _userRepository.TryGetAsync(email);

            var user = await _userRepository.GetAsync(u => u.Email == email);

            if (user == null)
                throw new ValidationException(ErrorMessages.INVALID_EMAIL);

            var resetToken = Guid.NewGuid().ToString();
            _memoryCache.Set(resetToken, user.Id, TimeSpan.FromMinutes(_resetSettings.TokenExpiryMinutes));

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
            var resetLink = $"{_resetSettings.ResetLinkBaseUrl}?token={resetToken}";
            return resetLink;
        }
    }
}