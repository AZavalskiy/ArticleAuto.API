using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.Domain.ServiceConfigurations
{
    public class PasswordResetSettings
    {
        public double TokenExpiryMinutes { get; set; }

        public string ResetLinkBaseUrl { get; set; }
    }
}
